using Microsoft.EntityFrameworkCore;

namespace CarWash_App.Services
{
    public class MyHostedService : IHostedService, IDisposable
    {
        private readonly IServiceProvider serviceProvider;
        private Timer FinishedServicesTimer;
        private Timer ServiceCancelationTimer;
        private Timer OldServicesRemoveTimer;

        public MyHostedService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public void Dispose()
        {

            FinishedServicesTimer?.Dispose();
            ServiceCancelationTimer?.Dispose();
            OldServicesRemoveTimer?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            FinishedServicesTimer = new Timer(FinishedServicesUpdate, null, TimeSpan.Zero, TimeSpan.FromSeconds(30));
            ServiceCancelationTimer = new Timer(CancelationServiceUpdate, null, TimeSpan.Zero, TimeSpan.FromSeconds(30));
            OldServicesRemoveTimer = new Timer(OldServiceRemoveUpdate, null, TimeSpan.Zero, TimeSpan.FromSeconds(30));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            FinishedServicesTimer?.Change(Timeout.Infinite, 0);
            ServiceCancelationTimer?.Change(Timeout.Infinite, 0);
            OldServicesRemoveTimer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        private async void FinishedServicesUpdate(object state)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var services = await context.Services
                    .Include(sc => sc.ServiceType)
                    .ToListAsync();

                var finishedServices = services.Where(sc => DateTime.Now > sc.ScheduledTime.Add(sc.ServiceType.Duration)
                                                         && sc.PaymentCollected == false);

                var aggregated = finishedServices
                    .GroupBy(sc => sc.CarWashId)
                    .Select(scGroup => new
                    {
                        carWashId = scGroup.Key,
                        SumOfRevenue = scGroup.Sum(x => x.ServiceType.Cost)
                    });

                var carWashes = await context.carWashes
                    .ToListAsync();

                foreach (var service in aggregated)
                {
                    var carWash = carWashes.Find(cw => cw.Id == service.carWashId);
                    carWash.Profit += service.SumOfRevenue;
                }
                foreach (var service in finishedServices)
                {
                    service.PaymentCollected = true;
                    context.Update(service);
                }

                await context.SaveChangesAsync();
            }
        }

        private async void CancelationServiceUpdate(object state)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var services = await context.Services
                    .ToListAsync();

                var services2 = services.Where(sc => sc.ScheduledTime.Subtract(DateTime.Now) < TimeSpan.FromMinutes(15));

                foreach (var service in services2)
                {
                    service.EligibleForCancelation = false;
                }
                await context.SaveChangesAsync();
            }
        }

        private async void OldServiceRemoveUpdate(object state)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var lastMonth = DateTime.Now.Subtract(TimeSpan.FromDays(30));

                var servicesForRemoval = await context.Services
                    .Where(sc => sc.ScheduledTime < lastMonth)
                    .ToListAsync();

                foreach (var service in servicesForRemoval)
                {
                    context.Services.Remove(service);
                }
                await context.SaveChangesAsync();

            }

        }
    }

}
