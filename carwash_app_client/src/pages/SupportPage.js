import React from "react";
import {makeStyles} from "@material-ui/core/styles";
import topBlockBackground from "../assets/images/support_tom.jpg";
import {Grid} from "@material-ui/core";

const useStyles = makeStyles(theme => ({
  topBlock: {
    width: "40%",
    backgroundImage: `url(${topBlockBackground})`,
    height: "40rem",
    backgroundSize: "contain",
    backgroundRepeat:"no-repeat",
    backgroundPositionY: "bottom",
    marginTop: "-0.5rem",
    marginLeft: "auto",
    marginRight: "auto",
    // [theme.breakpoints.down("xs")]: {
    //   backgroundImage: `url(${topBlockBackgroundMobile})`,
    // },
  },
}));

const SupportPage = (props) => {
  const classes = useStyles();

  return ( <Grid direction="column" container>
    <Grid
      item
      container
      className={classes.topBlock}
      justifyContent="space-between"
    ></Grid>
  </Grid>);
};

export default SupportPage;