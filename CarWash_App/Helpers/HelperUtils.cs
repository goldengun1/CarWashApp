using CarWash_App.DTOs.CarWashDTOs;
using CarWash_App.DTOs.ServiceDTOs;
using CarWash_App.Entities;
using System.Security.Claims;

namespace CarWash_App.Helpers
{
    public class HelperUtils
    {
        public static CarWashStatsDTO CalculateCarWashStatistics(CarWash carWash)
        {

            var totalScheduled = carWash.Services.Count(sc => sc.ScheduledTime > DateTime.Now);
            var confirmedServices = carWash.Services.Where(x => x.Confirmed && x.ScheduledTime > DateTime.Now);
            var totalConfirmed = confirmedServices.Count();
            var toBeCharged = (float)confirmedServices.Sum(x => x.ServiceType.Cost);



            return new CarWashStatsDTO() 
            {
                Id = carWash.Id,
                OwnerId = carWash.OwnerId,
                Profit = carWash.Profit + " $",
                ToBeCharged = toBeCharged + " $",
                TotalScheduled = totalScheduled,
                CofirmedServices = totalConfirmed,
                AverageProfitPerService = (toBeCharged/(totalConfirmed != 0 ? totalConfirmed : 1)).ToString() + " $"
            };
        }

        public static bool CredsValidated(ClaimsPrincipal currUser, string email)
        {
            if (currUser.IsInRole("Admin"))
                return true;

            var userEmail = currUser.FindFirst(ClaimTypes.Email)?.Value;
            return userEmail != null ? userEmail.Equals(email) : false;
        }

        public static bool CheckAvailability(CarWash carWash, ServiceCreationDTO serviceCreationDTO, ServiceType serviceType)
        {
            //initialization
            var valid = true;
            var array = new List<int>(new int[24]);
            var cwServices = carWash.Services;
            var sch_service = (Hour: serviceCreationDTO.ScheduledTime.Hour, Duraton: Convert.ToInt32(serviceType.Duration.TotalHours));

            //date check
            if (DateTime.Now > serviceCreationDTO.ScheduledTime)
                return false;

            //working hours check
            if (sch_service.Hour < carWash.OpeningTime || carWash.ClosingTime < (sch_service.Hour + sch_service.Duraton))
                return false;

            //no services check
            if (cwServices == null || !cwServices.Any())
                return true;

            //calculating existing services
            foreach (var service in cwServices)
            {
                var hour = service.ScheduledTime.Hour;
                var duration = Convert.ToInt32(service.ServiceType.Duration.TotalHours);
                for (int i = hour; i < hour + duration; i++)
                {
                    array[i] += 1;
                }
            }
            //calculating scheduled service
            for (int i = sch_service.Hour; i < sch_service.Hour + sch_service.Duraton; i++)
            {
                array[i] += 1;
            }

            //availability check
            foreach (var hour in array)
            {
                if (hour > 1)
                    valid = false;
            }
            return valid;
        }

    }
}
