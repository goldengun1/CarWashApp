import React, { useContext, useEffect, useState } from "react";
import { makeStyles } from "@material-ui/core/styles";
import {
  Button,
  CircularProgress,
  Dialog,
  DialogActions,
  DialogContent,
  FormControl,
  Grid,
  InputLabel,
  LinearProgress,
  List,
  ListItem,
  MenuItem,
  Select,
  Slide,
  Snackbar,
  TextField,
  Typography,
  useMediaQuery,
} from "@material-ui/core";
import Lottie from "react-lottie";
import topBlockBackground from "../assets/images/car_wash_gtr.jpg";
import topBlockBackgroundMobile from "../assets/images/car_wash_mobile.jpg";
import mainBannerAnimation from "../assets/animations/washing_car_drivethrough_animation.json";
import theme from "../components/ui/Theme";
import CarWashListItem from "../components/CarWash/CarWashListItem";
import AuthContext from "../context/auth-context";
import MyShopListItem from "../components/CarWash/MyShopListItem";
import { Pagination } from "@material-ui/lab";

const DUMMY_CAR_WASHES = [
  {
    offeredServices: [
      {
        id: 1,
        serviceName: "Regular",
        duration: "01:00:00",
        cost: "2.5 $",
      },
      {
        id: 2,
        serviceName: "Extended",
        duration: "02:00:00",
        cost: "4.5 $",
      },
      {
        id: 3,
        serviceName: "Premium",
        duration: "03:00:00",
        cost: "8.75 $",
      },
    ],
    id: 1,
    carWashName: "CarWashExtra",
    openingTime: 9,
    closingTime: 17,
  },
  {
    offeredServices: [
      {
        id: 1,
        serviceName: "Regular",
        duration: "01:00:00",
        cost: "2.5 $",
      },
      {
        id: 2,
        serviceName: "Extended",
        duration: "02:00:00",
        cost: "4.5 $",
      },
    ],
    id: 2,
    carWashName: "MegaWash",
    openingTime: 12,
    closingTime: 22,
  },
  {
    offeredServices: [
      {
        id: 2,
        serviceName: "Extended",
        duration: "02:00:00",
        cost: "4.5 $",
      },
      {
        id: 3,
        serviceName: "Premium",
        duration: "03:00:00",
        cost: "8.75 $",
      },
    ],
    id: 3,
    carWashName: "StecoPoint",
    openingTime: 6,
    closingTime: 15,
  },
  {
    offeredServices: [
      {
        id: 1,
        serviceName: "Regular",
        duration: "01:00:00",
        cost: "2.5 $",
      },
    ],
    id: 4,
    carWashName: "4U2Wash",
    openingTime: 0,
    closingTime: 24,
  },
];

const useStyles = makeStyles((theme) => ({
  topBlock: {
    width: "100%",
    backgroundImage: `url(${topBlockBackground})`,
    // height: "30rem",
    backgroundSize: "cover",
    backgroundPositionY: "bottom",
    marginTop: "-0.5rem",
    [theme.breakpoints.down("xs")]: {
      backgroundImage: `url(${topBlockBackgroundMobile})`,
    },
  },
  mainBannerAnimation: {
    maxHeight: "30rem",
    // maxWidth: "30rem",
    backgroundColor: "#000000AA",
  },
  mainCarWashList: {
    width: "100%",
    marginBottom: "3rem",
  },
  formControl: {
    marginTop: theme.spacing(2),
    marginBottom: theme.spacing(2),
    minWidth: 200,
  },
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
    [theme.breakpoints.down("xs")]: {
      width: "8rem",
    },
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

const Transition = React.forwardRef(function Transition(props, ref) {
  return <Slide direction="up" ref={ref} {...props} />;
});

function SnackBarTransition(props) {
  return <Slide {...props} direction="down" />;
}

const HomePage = () => {
  //STATES
  const [carWashes, setCarWashes] = useState([]);
  const [isLoading, setIsLoading] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [schedulingCarWash, setSchedulingCarWash] = useState({});
  const [dialogIsOpen, setDialogIsOpen] = useState(false);
  const [serviceType, setServiceType] = useState("");
  const [scheduleServiceDate, setScheduleServiceDate] = useState(null);
  const [serviceTypeIsValid, setServiceTypeIsValid] = useState(true);
  const [serviceDateIsValid, setServiceDateIsValid] = useState(true);
  const [shouldReload, setShouldReload] = useState(false);
  const [snackBarShown, setSnackBarShown] = useState(false);
  const [snackBarMessage, setSnackBarMessage] = useState({
    severity: "Info",
    message: "",
  });
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(0);

  const classes = useStyles();
  const authCtx = useContext(AuthContext);
  const { token, isOwner } = authCtx;
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

  //VARS

  const ownerGreetingsMessages = [
    "As owner you can find all scheduled services in your shops.",
    "Here You can view,confirm or reject them.",
  ];
  const customerGreetingsMessages = [
    "Find the best car wash shop for your needs.",
    "Schedule washing services for your vehicle fast and easy.",
  ];

  const mainBannerAnimationOptions = {
    loop: true,
    autoplay: true,
    animationData: mainBannerAnimation,
    rendererSettings: {
      preserveAspectRatio: "xMidYMid slice",
    },
  };

  //HANDLERS
  const confirmAllServicesHandler = (id) => {
    const carwash = carWashes.find((cw) => cw.id === id);
    const services = carwash.scheduledServices.filter((svc) => !svc.confirmed);
    if (services.length === 0) {
      setSnackBarShown(true);
      setSnackBarMessage({
        severity: "Info",
        message: "No services to confirm",
      });
      return;
    }
    setIsSubmitting(true);
    fetch(`https://localhost:7092/api/carwash/${id}/confirmall`, {
      method: "POST",
      headers: {
        Authorization: `Bearer ${token}`,
      },
    })
      .then((response) => {
        if (response.ok) {
          // const modifiedCWs = carWashes.filter(cw => cw.id !== id);
          let cw = carWashes.find((cw) => cw.id === id);
          for (let svc of cw.scheduledServices) {
            svc.confirmed = true;
          }
          console.log(cw);
          setSnackBarShown(true);
          setSnackBarMessage({
            severity: "Success",
            message: "All services confirmed",
          });
          setIsSubmitting(false);
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
      .catch((err) => {
        setSnackBarShown(true);
        setSnackBarMessage({
          severity: "Error",
          message: "Could not confirm all services",
        });
        setIsSubmitting(false);
        console.log(err.message);
      });
  };

  const confirmSingleServiceHandler = (carWashId, serviceId) => {
    setIsSubmitting(true);
    const confirmServiceDTO = {
      carWashId: carWashId,
      serviceId: serviceId,
    };
    fetch("https://localhost:7092/api/carwash/confirmservice", {
      method: "POST",
      headers: {
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json",
      },
      body: JSON.stringify(confirmServiceDTO),
    })
      .then((response) => {
        if (response.ok) {
          let cw = carWashes.find((cw) => cw.id === carWashId);
          for (let svc of cw.scheduledServices) {
            if (svc.id === serviceId) {
              svc.confirmed = true;
            }
          }
          setSnackBarShown(true);
          setSnackBarMessage({
            severity: "Success",
            message: "Service confirmed",
          });
          setIsSubmitting(false);
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
      .catch((err) => {
        console.log(err.message);
        setSnackBarShown(true);
        setSnackBarMessage({
          severity: "Error",
          message: "Confirmation Failed",
        });
        setIsSubmitting(false);
      });
    console.log(`CONFIRMED SERVICE: ${serviceId} IN CARWASH: ${carWashId}`);
  };

  const rejectServiceHandler = (carWashId, serviceId) => {
    setIsSubmitting(true);
    const rejectServiceDTO = {
      carWashId: carWashId,
      serviceId: serviceId,
    };
    fetch("https://localhost:7092/api/carwash/rejectservice", {
      method: "DELETE",
      headers: {
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json",
      },
      body: JSON.stringify(rejectServiceDTO),
    })
      .then((response) => {
        if (response.ok) {
          setSnackBarShown(true);
          setSnackBarMessage({
            severity: "Success",
            message: "Succesfully rejected service",
          });
          setShouldReload((prev) => !prev);
          setIsSubmitting(false);
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
      .catch((err) => {
        console.log(err.message);
        setSnackBarShown(true);
        setSnackBarMessage({ severity: "Warning", message: `${err.message}` });
        setIsSubmitting(false);
      });
    console.log(`REJECTED SERVICE: ${serviceId} IN CARWASH: ${carWashId}`);
  };

  const scheduleServiceHandler = (event) => {
    event.preventDefault();
    setIsSubmitting(true);
    if (serviceType === "") {
      setServiceTypeIsValid(false);
      setIsSubmitting(false);
      return;
    }
    if (
      new Date(scheduleServiceDate).getTime() < new Date().getTime() ||
      scheduleServiceDate === null
    ) {
      setServiceDateIsValid(false);
      return;
    }

    const serviceCreationDTO = {
      carWashId: schedulingCarWash.id,
      serviceTypeId: serviceType,
      scheduledTime: scheduleServiceDate,
    };
    // console.log(serviceCreationDTO);

    fetch("https://localhost:7092/api/service", {
      method: "POST",
      headers: {
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json",
      },
      body: JSON.stringify(serviceCreationDTO),
    })
      .then((response) => {
        if (response.ok) {
          setIsSubmitting(false);
          setSnackBarShown(true);
          setSnackBarMessage({
            severity: "Success",
            message: "Service scheduled successfully",
          });
          setDialogIsOpen(false);
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
      .catch((err) => {
        setIsSubmitting(false);
        setSnackBarShown(true);
        setSnackBarMessage({ severity: "Error", message: `${err.message}` });
        setDialogIsOpen(false);
      });
  };

  const selectServiceTypeChangeHandler = (event) => {
    setServiceType(event.target.value);
    setServiceTypeIsValid(true);
  };

  const handleClose = (event, reason) => {
    if (reason === "clickaway") {
      return;
    }

    setSnackBarShown(false);
  };

  //FETCHONG DATA ON LOAD
  useEffect(() => {
    setIsLoading(true);
    let url = isOwner
      ? "https://localhost:7092/api/carwash/getmyshops"
      : `https://localhost:7092/api/carwash?Page=${page}&RecordsPerPage=3`;
    fetch(url, {
      method: "GET",
      headers: {
        Authorization: `Bearer ` + token,
      },
    })
      .then((response) => {
        if (response.ok) {
          setTotalPages(parseInt(response.headers.get("totalAmountPages")));
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
      .then((responseData) => {
        console.log(responseData);
        setCarWashes(responseData);
        setIsLoading(false);
      })
      .catch((err) => {
        alert(err.message);
        setIsLoading(false);
      });
  }, [shouldReload, page]);

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
  //Owner: Car Wash List
  const myShops = (
    <Grid
      item
      container
      className={classes.mainCarWashList}
      justifyContent="center"
    >
      {isLoading ? (
        <Grid item style={{ width: "100%", height: "30rem" }}>
          <LinearProgress color="primary" />
          <LinearProgress color="primary" />
          <LinearProgress color="secondary" />
          <LinearProgress color="secondary" />
        </Grid>
      ) : (
        <List style={{ width: "60rem", margin: "0 auto" }}>
          {carWashes.map((carwash, index) => (
            <ListItem key={index} divider disableGutters>
              <MyShopListItem
                isSubmitting={isSubmitting}
                confirmAllHandler={confirmAllServicesHandler}
                confirmHandler={confirmSingleServiceHandler}
                rejectHandler={rejectServiceHandler}
                carwash={carwash}
              />
            </ListItem>
          ))}
        </List>
      )}
    </Grid>
  );
  //Customer: Car Wash List
  const carWashList = (
    <Grid
      item
      container
      className={classes.mainCarWashList}
      justifyContent="center"
    >
      {isLoading ? (
        <Grid item style={{ width: "100%", height: "30rem" }}>
          <LinearProgress color="primary" />
          <LinearProgress color="primary" />
          <LinearProgress color="secondary" />
          <LinearProgress color="secondary" />
        </Grid>
      ) : (
        <>
          <Grid
            item
            container
            justifyContent={matchesXS ? "center" : "flex-end"}
            style={{marginTop:"2rem"}}
          >
            <Grid item style={{ marginRight: matchesXS ? undefined : "6rem" }}>
              <Pagination
                page={page}
                count={totalPages}
                color="secondary"
                onChange={(ev, val) => {
                  setPage(val);
                }}
              />
            </Grid>
          </Grid>
          <List style={{ width: "50rem", margin: "0 auto" }}>
            {carWashes.map((carwash, index) => (
              <ListItem key={index} divider disableGutters>
                <CarWashListItem
                  setSchedulingCarWash={setSchedulingCarWash}
                  setDialogIsOpen={setDialogIsOpen}
                  carwash={carwash}
                />
              </ListItem>
            ))}
          </List>
        </>
      )}
    </Grid>
  );
  //Dialog component
  const dialog = (
    <Dialog
      open={dialogIsOpen}
      onClose={() => {
        setDialogIsOpen(false);
        setServiceType("");
        setServiceTypeIsValid(true);
        setServiceDateIsValid(true);
      }}
      TransitionComponent={Transition}
      fullScreen={matchesXS ? true : false}
      style={{ zIndex: matchesXS ? theme.zIndex.modal + 1 : undefined }}
    >
      <DialogContent dividers>
        <Grid container direction="column">
          <Grid item>
            <Typography align="center" variant="h2">
              Schedule Appointment
            </Typography>
          </Grid>
          <Grid item container direction="column" style={{ marginTop: "2rem" }}>
            <Grid item>
              <Typography variant="h4">
                {schedulingCarWash.carWashName}
              </Typography>
            </Grid>
            <Grid item>
              <FormControl className={classes.formControl} fullWidth>
                <InputLabel id="service-type-select-label">
                  Service Type
                </InputLabel>
                <Select
                  fullWidth
                  error={!serviceTypeIsValid}
                  labelId="service-type-select-label"
                  id="service-type-select"
                  value={serviceType}
                  onChange={selectServiceTypeChangeHandler}
                >
                  {schedulingCarWash.offeredServices &&
                    schedulingCarWash.offeredServices.map((svcType) => (
                      <MenuItem key={svcType.id} value={svcType.id}>
                        <span>{svcType.serviceName}</span>
                        <span style={{ marginLeft: "auto" }}>
                          {svcType.cost}
                          {` \t`}({parseInt(svcType.duration)}h)
                        </span>
                      </MenuItem>
                    ))}
                </Select>
              </FormControl>
            </Grid>
            <Grid item>
              <TextField
                id="datetime-local"
                label="Service date and time(only full hours)"
                type="datetime-local"
                defaultValue="2022-11-21T12:00"
                //TODO: fix default date value
                // value={new Date().toLocaleTimeString()}
                onChange={(ev) => {
                  setScheduleServiceDate(ev.target.value);
                  setServiceDateIsValid(true);
                }}
                fullWidth
                error={!serviceDateIsValid}
                className={classes.textField}
                InputLabelProps={{
                  shrink: true,
                }}
              />
            </Grid>
          </Grid>
        </Grid>
      </DialogContent>
      <DialogActions>
        <Grid container justifyContent="space-between">
          <Grid item>
            <Button
              className={classes.scheduleButton}
              onClick={() => {
                setDialogIsOpen(false);
                setServiceType("");
                setServiceTypeIsValid(true);
                setServiceDateIsValid(true);
              }}
            >
              Cancel
            </Button>
          </Grid>
          <Grid item>
            <Button
              className={classes.scheduleButton}
              onClick={scheduleServiceHandler}
            >
              {isSubmitting ? <CircularProgress /> : "Schedule"}
            </Button>
          </Grid>
        </Grid>
      </DialogActions>
    </Dialog>
  );

  return (
    <Grid direction="column" container>
      <Grid
        item
        container
        className={classes.topBlock}
        justifyContent="space-between"
      >
        <Grid item lg={7}>
          {/* text */}
          <Grid
            container
            direction="column"
            style={{
              height: "100%",
              backgroundColor: "#000000AA",
              padding: "0 15px",
              paddingTop: matchesXS ? "1.5rem" : undefined,
            }}
            justifyContent="center"
            alignItems="center"
          >
            <Grid item>
              <Typography
                variant="h2"
                align="center"
                style={{ color: "white" }}
              >
                Welcome to CarWashAPI
              </Typography>
            </Grid>
            <Grid item>
              <Typography
                variant="h4"
                align="center"
                style={{ color: "white" }}
                paragraph
              >
                {isOwner
                  ? ownerGreetingsMessages[0]
                  : customerGreetingsMessages[0]}
              </Typography>
            </Grid>
            <Grid item>
              <Typography
                variant="body1"
                align="center"
                paragraph
                style={{ color: "white", textOverflow: "clip" }}
              >
                {isOwner
                  ? ownerGreetingsMessages[1]
                  : customerGreetingsMessages[1]}
              </Typography>
            </Grid>
          </Grid>
        </Grid>
        <Grid item className={classes.mainBannerAnimation} lg={5}>
          <Lottie options={mainBannerAnimationOptions} />
        </Grid>
      </Grid>
      {/*DIALOG COMPONENT*/}
      {dialog}
      {snackBar}
      {isOwner && myShops}
      {!isOwner && carWashList}
    </Grid>
  );
};

export default HomePage;
