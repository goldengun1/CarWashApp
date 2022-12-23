import {Button, CircularProgress, Grid, Typography, useMediaQuery} from "@material-ui/core";
import theme from "../ui/Theme";
import AttachMoneyIcon from "@material-ui/icons/AttachMoney";
import HomeIcon from "@material-ui/icons/Home";
import FingerprintIcon from "@material-ui/icons/Fingerprint";
import TodayIcon from "@material-ui/icons/Today";
import React from "react";
import { makeStyles } from "@material-ui/core/styles";

const useStyles = makeStyles((theme) => ({
  cancelReservationButton: {
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
      border: `1px solid ${theme.palette.common.oruby}`,
      backgroundColor: "#6e222e",
    },
  },
}));

const ServicesListItem = (props) => {
  const classes = useStyles();
  const { service,isCanceling } = props;
  const matchesXS = useMediaQuery(theme.breakpoints.down("xs"));
  // const matchesMD = useMediaQuery(theme.breakpoints.down("md"));

  //HANDLERS
  const cancelReservationHandler = (event) => {
    event.preventDefault();
    props.cancelServiceHandler(service.id);
  };

  return (
    <Grid
      container
      direction={matchesXS ? "column" : undefined}
      alignItems="center"
      justify="space-between"
    >
      <Grid item lg={7} style={{ paddingLeft: matchesXS ? 0 : "20px" }}>
        {/* left side(info) */}
        <Grid container direction="column">
          <Grid
            item
            container
            justify={matchesXS ? "center" : "flex-start"}
            alignItems="center"
          >
            <Grid item>
              <FingerprintIcon color="secondary" />
            </Grid>
            <Grid item>
              <Typography
                align={matchesXS ? "center" : undefined}
                style={{ color: "#444444" }}
                variant="h2"
              >
                {service.id}
              </Typography>
            </Grid>
          </Grid>
          <Grid
            item
            container
            justify={matchesXS ? "center" : "flex-start"}
            alignItems="center"
          >
            <Grid item>
              <HomeIcon color="secondary" />
            </Grid>
            <Grid item>
              <Typography
                align={matchesXS ? "center" : undefined}
                variant="h4"
                style={{ color: theme.palette.primary.main }}
              >
                {service.carWashName}
              </Typography>
            </Grid>
          </Grid>
          <Grid
            item
            container
            justify={matchesXS ? "center" : "flex-start"}
            alignItems="center"
          >
            <Grid item>
              <AttachMoneyIcon color="secondary" />
            </Grid>
            <Grid item>
              <Typography style={{ color: "grey" }} variant="body1">
                {service.serviceTypeInfo}
              </Typography>
            </Grid>
          </Grid>
          <Grid
            item
            container
            justify={matchesXS ? "center" : "flex-start"}
            alignItems="center"
          >
            <Grid item>
              <TodayIcon color="secondary" />
            </Grid>
            <Grid item>
              <Typography
                align={matchesXS ? "center" : undefined}
                style={{ color: "grey" }}
                variant="body1"
              >
                {new Date(service.scheduledTime).toLocaleString()}
              </Typography>
            </Grid>
          </Grid>
        </Grid>
      </Grid>
      <Grid item>
        <Button
          disabled={!service.eligibleForCancelation}
          className={classes.cancelReservationButton}
          onClick={cancelReservationHandler}
        >
          {isCanceling ? <CircularProgress color='primary'/> :"Cancel Reservation"}
        </Button>
      </Grid>
    </Grid>
  );
};

export default ServicesListItem;
