import {
  Button,
  CircularProgress,
  Grid,
  List,
  ListItem,
  Typography,
  useMediaQuery,
} from "@material-ui/core";
import theme from "../ui/Theme";
import { makeStyles } from "@material-ui/core/styles";
import AccessTimeIcon from "@material-ui/icons/AccessTime";
import ServiceForConfirmation from "../Services/ServiceForConfirmation";

const useStyles = makeStyles((theme) => ({
  confirmAllServicesBtn: {
    width: "10rem",
    height: "1rem",
    backgroundColor: theme.palette.common.sienna,
    color: "white",
    fontWeight: 700,
    borderRadius: "10px",
    fontFamily: "Segoe UI",
    padding: "25px",
    border: `1px solid ${theme.palette.common.sienna}`,

    margin: "2rem 1.5rem 0 1rem",
    "&:hover": {
      border: `1px solid #007c14`,
      backgroundColor: "#007c14",
    },
  },
}));

const MyShopListItem = (props) => {
  const classes = useStyles();
  const {
    carwash,
    confirmAllHandler,
    confirmHandler,
    rejectHandler,
    isSubmitting,
  } = props;
  const matchesXS = useMediaQuery(theme.breakpoints.down("xs"));

  return (
    <Grid container direction="column">
      <Grid item>
        <Grid
          container
          direction={matchesXS ? "column" : undefined}
          alignItems="center"
          justify="space-between"
        >
          <Grid item lg={7} style={{ paddingLeft: matchesXS ? 0 : "20px" }}>
            {/*left side(info)*/}
            <Grid container direction="column">
              <Grid item>
                <Typography
                  align={matchesXS ? "center" : undefined}
                  style={{ color: "#444444" }}
                  variant="h2"
                >
                  {carwash.carWashName}
                </Typography>
              </Grid>
              <Grid
                item
                container
                justify={matchesXS ? "center" : "flex-start"}
                alignItems="center"
              >
                <Grid item>
                  <AccessTimeIcon color="secondary" />
                </Grid>
                <Grid item>
                  <Typography style={{ color: "grey" }} variant="body2">
                    {carwash.openingTime}:00h - {carwash.closingTime}:00h
                  </Typography>
                </Grid>
              </Grid>
            </Grid>
            <Grid item>
              <Typography
                align={matchesXS ? "center" : undefined}
                variant="body1"
              >
                Total Reservations: {carwash.scheduledServices.length}
              </Typography>
            </Grid>
            <Grid item>
              <Typography
                align={matchesXS ? "center" : undefined}
                variant="body1"
              >
                Unconfirmed:{" "}
                {
                  carwash.scheduledServices.filter(
                    (svc) => svc.confirmed === false
                  ).length
                }
              </Typography>
            </Grid>
          </Grid>
          <Grid item className={classes.actions}>
            {/*right size(actions)*/}
            <Button
              className={classes.confirmAllServicesBtn}
              onClick={() => {
                confirmAllHandler(carwash.id);
              }}
            >
              {isSubmitting ? (
                <CircularProgress color="primary" />
              ) : (
                "Confirm All Services"
              )}
            </Button>
          </Grid>
        </Grid>
      </Grid>
      <Grid item>
        <Grid
          container
          direction="column"
          justify="center"
        >
          <List style={{ margin: matchesXS ? 0 : "0 0 0 3rem" }}>
            {carwash.scheduledServices.filter(svc => !svc.confirmed).map((service, index) => (
              <ListItem key={index} disableGutters divider>
                <ServiceForConfirmation
                  service={service}
                  confirm={confirmHandler}
                  reject={rejectHandler}
                />
              </ListItem>
            ))}
          </List>
        </Grid>
      </Grid>
    </Grid>
  );
};

export default MyShopListItem;
