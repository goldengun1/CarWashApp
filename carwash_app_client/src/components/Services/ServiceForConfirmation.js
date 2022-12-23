import React from "react";
import {Button, CircularProgress, Grid, IconButton, Typography, useMediaQuery} from "@material-ui/core";
import theme from "../ui/Theme";
import {makeStyles} from "@material-ui/core/styles";
import FingerprintIcon from "@material-ui/icons/Fingerprint";
import CheckIcon from '@material-ui/icons/Check';
import CheckCircleOutlinedIcon from '@material-ui/icons/CheckCircleOutlined';
import CheckCircleIcon from '@material-ui/icons/CheckCircle';
import CancelOutlinedIcon from '@material-ui/icons/CancelOutlined';
import CancelIcon from '@material-ui/icons/Cancel';
import TodayIcon from "@material-ui/icons/Today";

const useStyles = makeStyles(theme => ({
  confirmBtn:{
    width: "6rem",
    height: "1rem",
    border: `1px solid #009d19`,
    backgroundColor: "#009d19",
    color: "white",
    fontWeight: 700,
    borderRadius: "10px",
    fontFamily: "Segoe UI",
    padding: "25px",

     margin: "0rem 1.5rem 0 0rem",
    "&:hover": {
      border: `1px solid #005b0f`,
      backgroundColor: "#005b0f",
    },
  },
  rejectBtn:{
    width: "6rem",
    height: "1rem",
    border: `1px solid #d31919`,
    backgroundColor: "#d31919",
    color: "white",
    fontWeight: 700,
    borderRadius: "10px",
    fontFamily: "Segoe UI",
    padding: "25px",

     margin: "0rem 1.5rem 0 0rem",
    "&:hover": {
      border: `1px solid #6b0101`,
      backgroundColor: "#6b0101",
    },
  },
}));

const ServiceForConfirmation = (props) => {
  const classes = useStyles();
  const { service, confirm, reject} = props;
  const matchesXS = useMediaQuery(theme.breakpoints.down("xs"));

  //HANDLERS
  const confirmHandler = (event) => {
    event.preventDefault();
    confirm(service.carWashId, service.id);
  };

  const rejectHandler = (event) => {
    event.preventDefault();
    reject(service.carWashId, service.id);
  };

  return ( <Grid
    container
    direction={matchesXS ? "column" : undefined}
    alignItems="center"
    justify="space-between"
  >
    <Grid item lg={7} style={{ paddingLeft: matchesXS ? 0 : "10px" }}>
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
      <IconButton onClick={confirmHandler}>
        <CheckCircleIcon style={{color: "#009d19"}}/>
        Confirm
      </IconButton>
      <IconButton onClick={rejectHandler}>
        <CancelIcon style={{color: "#d31919"}}/>
        Reject
      </IconButton>
    </Grid>
  </Grid>);
};

export default ServiceForConfirmation;