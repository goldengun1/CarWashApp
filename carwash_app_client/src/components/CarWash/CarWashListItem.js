import { Button, Grid, Typography, useMediaQuery } from "@material-ui/core";
import { makeStyles } from "@material-ui/core/styles";
import AccessTimeIcon from "@material-ui/icons/AccessTime";
import theme from "../ui/Theme";

const useStyles = makeStyles((theme) => ({
  actions: {},
  scheduleButton: {
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
      border: `1px solid ${theme.palette.common.sienna}`,
      backgroundColor: "#a26a47",
    },
  },
}));

const CarWashListItem = (props) => {
  const classes = useStyles();
  const { carwash } = props;
  const matchesXS = useMediaQuery(theme.breakpoints.down("xs"));

  const scheduleHandler = (event) => {
    props.setSchedulingCarWash(carwash);
    props.setDialogIsOpen(true);
  };

  return (
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
          <Typography align={matchesXS ? "center" : undefined} variant="body1">
            Offered Services: {carwash.offeredServices.length}
          </Typography>
        </Grid>
      </Grid>
      <Grid item className={classes.actions}>
        {/*right size(actions)*/}
        <Button className={classes.scheduleButton} onClick={scheduleHandler}>
          Request Service
        </Button>
      </Grid>
    </Grid>
  );
};

export default CarWashListItem;
