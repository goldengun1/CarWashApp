import { makeStyles } from "@material-ui/core/styles";
import {
  Grid,
  LinearProgress,
  List,
  ListItem, Slide, Snackbar,
  Typography,
  useMediaQuery,
} from "@material-ui/core";
import React, {useContext, useEffect, useState} from "react";
import theme from "../components/ui/Theme";
import topBlockBackground from "../assets/images/calendar_clock_wallpaper.jpg";
import ServicesListItem from "../components/Services/ServicesListItem";
import AuthContext from "../context/auth-context";

const DUMMY_SERVICES = [
  {
    customer: "Ana Anic",
    serviceTypeInfo: "Regular 2.5$",
    carWashName: "CarWashExtra",
    id: 7,
    carWashId: 1,
    customerId: "722D96C6-60D1-4233-B41E-D788E2451D4F",
    serviceTypeId: 1,
    scheduledTime: "2022-12-12T12:00:00",
    eligibleForCancelation: true,
    confirmed: false,
  },
  {
    customer: "Ana Anic",
    serviceTypeInfo: "Regular 2.5$",
    carWashName: "CarWashExtra",
    id: 8,
    carWashId: 1,
    customerId: "722D96C6-60D1-4233-B41E-D788E2451D4F",
    serviceTypeId: 1,
    scheduledTime: "2022-12-12T10:00:00",
    eligibleForCancelation: true,
    confirmed: false,
  },
  {
    customer: "Ana Anic",
    serviceTypeInfo: "Regular 2.5$",
    carWashName: "CarWashExtra",
    id: 9,
    carWashId: 1,
    customerId: "722D96C6-60D1-4233-B41E-D788E2451D4F",
    serviceTypeId: 1,
    scheduledTime: "2022-11-22T15:00:00",
    eligibleForCancelation: false,
    confirmed: false,
  },
  {
    customer: "Ana Anic",
    serviceTypeInfo: "Regular 2.5$",
    carWashName: "CarWashExtra",
    id: 10,
    carWashId: 1,
    customerId: "722D96C6-60D1-4233-B41E-D788E2451D4F",
    serviceTypeId: 1,
    scheduledTime: "2022-11-23T13:00:00",
    eligibleForCancelation: true,
    confirmed: false,
  },
  {
    customer: "Ana Anic",
    serviceTypeInfo: "Regular 2.5$",
    carWashName: "CarWashExtra",
    id: 11,
    carWashId: 1,
    customerId: "722D96C6-60D1-4233-B41E-D788E2451D4F",
    serviceTypeId: 1,
    scheduledTime: "2022-11-30T12:00:00",
    eligibleForCancelation: true,
    confirmed: false,
  },
  {
    customer: "Ana Anic",
    serviceTypeInfo: "Regular 2.5$",
    carWashName: "CarWashExtra",
    id: 12,
    carWashId: 1,
    customerId: "722D96C6-60D1-4233-B41E-D788E2451D4F",
    serviceTypeId: 1,
    scheduledTime: "2022-11-25T12:00:00",
    eligibleForCancelation: true,
    confirmed: false,
  },
  {
    customer: "Ana Anic",
    serviceTypeInfo: "Regular 2.5$",
    carWashName: "MegaWash",
    id: 13,
    carWashId: 2,
    customerId: "722D96C6-60D1-4233-B41E-D788E2451D4F",
    serviceTypeId: 1,
    scheduledTime: "2022-11-23T12:00:00",
    eligibleForCancelation: true,
    confirmed: false,
  },
];

const useStyles = makeStyles((theme) => ({
  topBlock: {
    width: "100%",
    backgroundImage: `url(${topBlockBackground})`,
    height: "30rem",
    backgroundSize: "cover",
    backgroundPositionY: "bottom",
    marginTop: "-0.5rem",
    [theme.breakpoints.down("xs")]: {
      backgroundImage: `url(${topBlockBackground})`,
    },
  },
  servicesList: {
    width: "100%",
    marginBottom: "3rem",
  },
  snackBarSuccess: {
    backgroundColor: "#4caf50",
    color: "white",
    fontFamily: "Segoe UI",
    textAlign: "center",
    fontSize: "1rem",
    fontWeight: "bold",
  },
  snackBarError: {
    backgroundColor: "#f44336",
    color: "white",
    fontFamily: "Segoe UI",
    textAlign: "center",
    fontSize: "1rem",
    fontWeight: "bold",
  },
  snackBarWarning: {
    backgroundColor: "#ff9800",
    color: "white",
    fontFamily: "Segoe UI",
    textAlign: "center",
    fontSize: "1rem",
    fontWeight: "bold",
  },
  snackBarInfo: {
    backgroundColor: "#2196f3",
    color: "white",
    fontFamily: "Segoe UI",
    textAlign: "center",
    fontSize: "1rem",
    fontWeight: "bold",
  },
}));

function SnackBarTransition(props) {
  return <Slide {...props} direction="down" />;
}

const MyReservationsPage = (props) => {

  //STATES
  const [services, setServices] = useState([]);
  const [isLoading, setIsLoading] = useState(false);
  const [isCanceling, setIsCanceling] = useState(false);
  const [snackBarShown, setSnackBarShown] = useState(false);
  const [snackBarMessage, setSnackBarMessage] = useState({
    severity: "Info",
    message: "",
  });

  const classes = useStyles();
  const authCtx = useContext(AuthContext);
  const {token} = authCtx;
  const matchesXS = useMediaQuery(theme.breakpoints.down("xs"));

  let snackBarClass = {};
  switch (snackBarMessage.severity) {
    case "Error":
      snackBarClass = {
        classes: {
          root: classes.snackBarError,
        },
      };
      break;
    case "Success":
      snackBarClass = {
        classes: {
          root: classes.snackBarSuccess,
        },
      };
      break;
    case "Warning":
      snackBarClass = {
        classes: {
          root: classes.snackBarWarning,
        },
      };
      break;
    case "Info":
      snackBarClass = {
        classes: {
          root: classes.snackBarInfo,
        },
      };
      break;
  }

  //HANDLERS
  const handleClose = (event, reason) => {
    if (reason === "clickaway") {
      return;
    }

    setSnackBarShown(false);
  };

  const cancelServiceHandler = (id) => {
    setIsCanceling(true);
    fetch(`https://localhost:7092/api/service/cancelService/${id}`,{
      method: "DELETE",
      headers: {
        Authorization: `Bearer ${token}`,
      },
    })
      .then(response => {
        if(response.ok){
          let newServices = services.filter(svc => svc.id !== id);
          setIsCanceling(false);
          setServices([...newServices]);
          setSnackBarShown(true);
          setSnackBarMessage({severity: "Success",message: "Service canceled successfully"});
        }
        else{
          return response.json().then(data => {
            console.log(data);
            let errorMessage = "Action Failed";
            if (data && data.value) {
              errorMessage = data.value;
            }
            throw new Error(errorMessage);
          });
        }
      })
      .catch(err => {
        console.log(err.message);
        setIsCanceling(false);
        setSnackBarShown(true);
        setSnackBarMessage({severity: "Error",message: `${err.message}`});
      });
  };

  //FETCH SERVICES ON LOAD
  useEffect(() => {
    setIsLoading(true);
    fetch('https://localhost:7092/api/service/getmyservices',{
      method: "GET",
      headers: {
        Authorization: `Bearer ${token}`,
        'Content-Tyopes': 'application/json',
      },
    })
      .then((response) => {
        if (response.ok) {
          return response.json();
        } else {
          return response.json().then((data) => {
            let errorMessage = "Action Failed";
            if (data && data.value) {
              errorMessage = data.value;
            }
            throw new Error(errorMessage);
          });
        }
      })
      .then(responseData => {
        const servicesForShowing = responseData.filter(service => new Date(service.scheduledTime).getTime() >= new Date().getTime());
        setServices(servicesForShowing);
        setIsLoading(false);
      })
      .catch(err => {
        console.log(err.message);
      });
  },[]);

  //JSX ELEMENTS
  const snackBar = (
    <Snackbar
      open={snackBarShown}
      message={snackBarMessage.message}
      ContentProps={snackBarClass}
      anchorOrigin={{ vertical: "top", horizontal: "center" }}
      autoHideDuration={3000}
      TransitionComponent={SnackBarTransition}
      onClose={handleClose}
    />
  );


  return (
    <Grid container direction="column">
      <Grid item container className={classes.topBlock} justify="space-between">
        <Grid item lg={5}>
          {/* text */}
          <Grid
            container
            direction="column"
            style={{
              height: "100%",
              backgroundColor: "#00000033",
              padding: "0 15px",
              paddingTop: matchesXS ? "1.5rem" : undefined,
            }}
            justify="center"
            alignItems="center"
          >
            <Grid item>
              <Typography
                variant="h2"
                align="center"
                style={{ color: "white" }}
              >
                Time is Money
              </Typography>
            </Grid>
            <Grid item>
              <Typography
                variant="h4"
                align="center"
                style={{ color: "white" }}
                paragraph
              >
                So dont waste any, and have easy access to all of your
                appointments.
              </Typography>
            </Grid>
            <Grid item>
              <Typography
                variant="body1"
                align="center"
                paragraph
                style={{ color: "white", textOverflow: "clip" }}
              >
                Manage appointments, view info or cancel reservation if You
                want, all here.
              </Typography>
            </Grid>
          </Grid>
        </Grid>
      </Grid>
      <Grid item container className={classes.servicesList} justify="center">
        {isLoading ? (
          <Grid item style={{ width: "100%", height: "30rem" }}>
            <LinearProgress color="primary" />
            <LinearProgress color="primary" />
            <LinearProgress color="secondary" />
            <LinearProgress color="secondary" />
          </Grid>
        ) : (
          <List style={{ width: "50rem", margin: "0 auto" }}>
            {services.map((service, index) => (
              <ListItem key={index} divider disableGutters>
                <ServicesListItem key={index} service={service} isCanceling={isCanceling} cancelServiceHandler={cancelServiceHandler}/>
              </ListItem>
            ))}
          </List>
        )}
      </Grid>
      {snackBar}
    </Grid>
  );
};

export default MyReservationsPage;
