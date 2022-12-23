import {
  Avatar, Button,
  Chip,
  Grid, IconButton,
  Typography,
  useMediaQuery,
} from "@material-ui/core";
import theme from "../ui/Theme";
import { makeStyles } from "@material-ui/core/styles";
import AccessTimeIcon from "@material-ui/icons/AccessTime";
import LocalOfferIcon from "@material-ui/icons/LocalOffer";
import EditIcon from '@material-ui/icons/Edit';
import DeleteIcon from '@material-ui/icons/Delete';
import DeleteForeverIcon from '@material-ui/icons/DeleteForever';
import AttachMoneyIcon from '@material-ui/icons/AttachMoney';
import InfoOutlinedIcon from '@material-ui/icons/InfoOutlined';
import AddIcon from "@material-ui/icons/Add";

const useStyles = makeStyles((theme) => ({
  avatar: {
    backgroundColor: theme.palette.secondary.main,
  }
}));

const CarWashDetails = (props) => {
  const classes = useStyles();
  const { carwash } = props;
  const matchesXS = useMediaQuery(theme.breakpoints.down("xs"));

  const editCarWashHandler = (event) => {
    event.preventDefault();
    props.setDetailedCarWash(carwash);
    props.setEdit(true);
    props.setEditDialogIsOpen(true);
    console.log("EDITING CAR WASH " + carwash.carWashName);
  };

  const getRevenueHandler = (event) => {
    event.preventDefault();
    props.setDetailedCarWash(carwash);
    props.setRevenueDialogIsOpen(true);
  };

  const deleteShopHandler = (event) => {
    event.preventDefault();
    props.setDetailedCarWash(carwash);
    props.setConfirmDialogIsOpen(true);
  };

  const getShopInfoHandler = (event) => {
    event.preventDefault();
    props.setDetailedCarWash(carwash);
    props.setCarWashInfoDialogIsOpen(true);
    props.getCarWashStats(carwash.id);
    console.log("STATS FOR CAR WASH " + carwash.carWashName);
  };

  const addSeerviceTypeHandler = (event) => {
    event.preventDefault();
    props.setDetailedCarWash(carwash);
    props.setServiceTypeDialogIsOpen(true);
    console.log("ADD SERVICE TYPE FOR CARWASH " + carwash.carWashName);
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
              style={{ color: "#555555" }}
              variant="h2"
            >
              {carwash.carWashName}
              <IconButton
                style={{ marginLeft: "10px" }}
                onClick={editCarWashHandler}
              >
                <EditIcon />
              </IconButton>
              <IconButton onClick={getShopInfoHandler}>
                <InfoOutlinedIcon />
              </IconButton>
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
            <Grid item style={{marginLeft: "10px"}}>
              <Typography style={{ color: "grey" }} variant="body2">
                {carwash.openingTime}:00h - {carwash.closingTime}:00h
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
              <LocalOfferIcon color="secondary" />
            </Grid>
            <Grid item>
              {carwash.offeredServices.map((svc) => (
                <Chip
                  style={{ marginLeft: "0.5rem" }}
                  classes={{ avatar: classes.avatar }}
                  variant="outlined"
                  key={svc.id}
                  label={svc.serviceName}
                  avatar={<Avatar>{svc.serviceName[0]}</Avatar>}
                />
              ))}
              <IconButton onClick={addSeerviceTypeHandler}>
                <AddIcon color="primary" />
              </IconButton>
            </Grid>
          </Grid>
        </Grid>
      </Grid>
      <Grid item className={classes.actions}>
        {/*right size(actions)*/}
        <Grid container>
          <IconButton onClick={getRevenueHandler}>
            <AttachMoneyIcon style={{ color: "green" }} />
            Revenue
          </IconButton>
          <Grid item>
            <IconButton onClick={deleteShopHandler}>
              <DeleteForeverIcon style={{ color: "#CC0000" }} />Delete
            </IconButton>
          </Grid>
        </Grid>
      </Grid>
    </Grid>
  );
};

export default CarWashDetails;
