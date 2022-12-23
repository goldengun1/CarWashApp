import { makeStyles } from "@material-ui/core/styles";
import topBlockBackground from "../assets/images/carwash_booth.jpg";
import {
  Button,
  CircularProgress,
  Dialog,
  DialogActions,
  DialogContent,
  FormControl,
  Grid, IconButton,
  InputAdornment,
  InputLabel,
  LinearProgress,
  List,
  ListItem,
  MenuItem,
  Select,
  Slide, Snackbar,
  TextField,
  Typography,
  useMediaQuery,
  Zoom,
} from "@material-ui/core";
import Fab from "@material-ui/core/Fab";
import AddIcon from "@material-ui/icons/Add";
import React, { useContext, useEffect, useRef, useState } from "react";
import theme from "../components/ui/Theme";
import AuthContext from "../context/auth-context";
import CarWashDetails from "../components/CarWash/CarWashDetails";
import MonetizationOnOutlinedIcon from '@material-ui/icons/MonetizationOnOutlined';
import TimerOutlinedIcon from '@material-ui/icons/TimerOutlined';
import TextFieldsOutlinedIcon from '@material-ui/icons/TextFieldsOutlined';
import ArrowBackIosOutlinedIcon from '@material-ui/icons/ArrowBackIosOutlined';
import AddOutlinedIcon from '@material-ui/icons/AddOutlined';

const useStyles = makeStyles((theme) => ({
  topBlock: {
    width: "100%",
    backgroundImage: `url(${topBlockBackground})`,
    height: "30rem",
    backgroundSize: "cover",
    backgroundPositionY: "center",
    marginTop: "-0.5rem",
    [theme.breakpoints.down("xs")]: {
      backgroundImage: `url(${topBlockBackground})`,
    },
  },
  carWashesList: {
    width: "100%",
    marginBottom: "3rem",
  },
  dialogButton: {
    width: "10rem",
    height: "1rem",
    backgroundColor: theme.palette.common.sienna,
    color: "white",
    fontWeight: 700,
    borderRadius: "10px",
    fontFamily: "Segoe UI",
    padding: "25px",
    border: `1px solid ${theme.palette.common.sienna}`,

    margin: "0.5rem 1rem",
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

const MyShopsPage = (props) => {
  const classes = useStyles();
  const matchesXS = useMediaQuery(theme.breakpoints.down("xs"));
  const authCtx = useContext(AuthContext);
  const { token } = authCtx;

  const [carWashes, setCarWashes] = useState([]);
  const [detailedCarWash, setDetailedCarWash] = useState({});
  const [revenueResponse, setRevenueResponse] = useState(null);
  const [carWashStatsResponse, setCarWashStatsResponse] = useState({});
  const [revenueFilterValue, setRevenueFilterValue] = useState(3);
  const [isLoading, setIsLoading] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [editDialogIsOpen, setEditDialogIsOpen] = useState(false);
  const [edit, setEdit] = useState(false);
  const [revenueDialogIsOpen, setRevenueDialogIsOpen] = useState(false);
  const [confirmDialogIsOpen, setConfirmDialogIsOpen] = useState(false);
  const [serviceTypeDialogIsOpen, setServiceTypeDialogIsOpen] = useState(false);
  const [carWashInfoDialogIsOpen, setCarWashInfoDialogIsOpen] = useState(false);
  const [shouldReload, setShouldReload] = useState(false);
  const [snackBarShown, setSnackBarShown] = useState(false);
  const [snackBarMessage, setSnackBarMessage] = useState({
    severity: "Info",
    message: "",
  });
  const carWashNameRef = useRef();
  const openingTimeRef = useRef();
  const closingTimeRef = useRef();
  const serviceTypeNameRef = useRef();
  const durationRef = useRef();
  const priceRef = useRef();

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

  const   addNewServiceTypeHandler = () => {
    setIsSubmitting(true);
    const serviceTypeName = serviceTypeNameRef.current.value;
    const serviceTypeDuration = durationRef.current.value;
    const serviceTypePrice = priceRef.current.value;
    let serviceTypeIds = detailedCarWash.offeredServices.map((svcType) => svcType.id);

    const serviceTypeCreationDTO = {
      serviceName: serviceTypeName,
      duration: serviceTypeDuration,
      cost: serviceTypePrice,
      carWashIds: [detailedCarWash.id]
    };

    fetch('https://localhost:7092/api/servicetype',{
      method: "POST",
      headers:{
        Authorization: `Bearer ${token}`,
        "Content-Type": 'application/json',
      },
      body: JSON.stringify(serviceTypeCreationDTO),
    })
      .then(response => {
        if(response.ok){
          return response.json();
        }
        else{
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
        console.log(responseData);
        serviceTypeIds = [...serviceTypeIds,responseData.id];
        setShouldReload(prev => !prev);
        setServiceTypeDialogIsOpen(false);
        setSnackBarShown(true);
        setSnackBarMessage({severity:"Success",message:`Added new service type "${serviceTypeName}"`});
        setIsSubmitting(false);
      })
      .catch(err => {
        console.log(err.message);
        setSnackBarShown(true);
        setSnackBarMessage({severity:"Error",message:`${err.message}`});
        setIsSubmitting(false);
      });

  };

  const getCarWashStatsHandler = (id) => {
    setIsSubmitting(true);
    fetch(`https://localhost:7092/api/carwash/${id}/stats`, {
      method: "GET",
      headers: {
        Authorization: `Bearer ${token}`,
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
      .then((responseData) => {
        setCarWashStatsResponse(responseData);
        setIsSubmitting(false);
      })
      .catch((err) => {
        console.log(err.message);
        setIsSubmitting(false);
      });
  };

  const deleteCarWashHandler = () => {
    setIsLoading(true);
    fetch(`https://localhost:7092/api/carwash/${detailedCarWash.id}`, {
      method: "DELETE",
      headers: {
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json",
      },
    })
      .then((response) => {
        if (response.ok) {
          //delete car wash from list
          const newList = carWashes.filter(
            (cw) => cw.id !== detailedCarWash.id
          );
          setCarWashes(newList);
          setSnackBarShown(true);
          setSnackBarMessage({severity:"Success",message:"Shop deleted successfully"});
          setIsLoading(false);
          setConfirmDialogIsOpen(false);
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
        setIsLoading(false);
        setConfirmDialogIsOpen(false);
        setSnackBarShown(true);
        setSnackBarMessage({severity:"Error",message:`${err.message}`});
      });
  };

  const editCarWashHandler = () => {
    setIsSubmitting(true);
    const serviceTypes = edit
      ? detailedCarWash.offeredServices.map((svcType) => svcType.id)
      : [];
    const method = edit ? "PUT" : "POST";
    const editDTO = {
      id: detailedCarWash.id,
      carWashName: carWashNameRef.current.value,
      openingTime: openingTimeRef.current.value,
      closingTime: closingTimeRef.current.value,
      serviceTypeIds: serviceTypes,
    };
    fetch("https://localhost:7092/api/carwash", {
      method: method,
      headers: {
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json",
      },
      body: JSON.stringify(editDTO),
    })
      .then((response) => {
        if (response.ok) {
          setShouldReload((prev) => !prev);
          setIsSubmitting(false);
          setSnackBarShown(true);
          setSnackBarMessage({severity:`${edit ? "Info" : "Success"}`,message:`Shop ${edit ? "edited" : "created"} successfully`});
          setEditDialogIsOpen(false);
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
        setIsSubmitting(false);
        setEditDialogIsOpen(false);
        setSnackBarShown(true);
        setSnackBarMessage({severity:"Error",message:"Shop editing failed"});
      });
  };

  const getCarWashRevenueHandler = () => {
    setIsSubmitting(true);
    const filter =
      revenueFilterValue === 1
        ? "?Daily=true"
        : revenueFilterValue === 2
        ? "?Weekly=true"
        : "?Monthly=true";

    fetch(
      `https://localhost:7092/api/carwash/revenueoverview/${detailedCarWash.id}${filter}`,
      {
        method: "GET",
        headers: {
          Authorization: `Bearer ${token}`,
        },
      }
    )
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
      .then((responseData) => {
        console.log(responseData.value);
        setRevenueResponse(responseData.value);
        setIsSubmitting(false);
      })
      .catch((err) => {
        console.log(err.message);
        setIsSubmitting(false);
      });
  };

  //FETCHING ON LOAD
  useEffect(() => {
    setIsLoading(true);
    fetch("https://localhost:7092/api/carwash/getmyshops", {
      method: "GET",
      headers: {
        Authorization: `Bearer ` + token,
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
      .then((responseData) => {
        console.log(responseData);
        setCarWashes(responseData);
        setIsLoading(false);
      })
      .catch((err) => {
        alert(err.message);
        setIsLoading(false);
      });
  }, [shouldReload]);

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

  const carWashInfoDialog = (
    <Dialog
      open={carWashInfoDialogIsOpen}
      onClose={() => {
        setCarWashInfoDialogIsOpen(false);
      }}
      TransitionComponent={Transition}
      fullScreen={matchesXS ? true : false}
      style={{ zIndex: matchesXS ? theme.zIndex.modal + 1 : undefined }}
    >
      <DialogContent dividers style={{ paddingBottom: "2rem" }}>
        <Grid container direction="column">
          <Grid item>
            <Typography
              align="center"
              variant="h2"
              style={{ marginBottom: "2rem" }}
            >
              Shop stats for{" "}
              <span style={{ color: theme.palette.primary.main }}>
                {detailedCarWash.carWashName}
              </span>
            </Typography>
          </Grid>
          {isSubmitting ? (
            <>
              <LinearProgress />
              <LinearProgress />
            </>
          ) : (
            <Grid item container direction="column" spacing={3}>
              <Grid item>
                <Grid container justify="space-between">
                  <Grid item>
                    <Typography variant="h5">Total scheduled:</Typography>
                  </Grid>
                  <Grid item>
                    <Typography variant="body1">
                      {carWashStatsResponse.totalScheduled}
                    </Typography>
                  </Grid>
                </Grid>
              </Grid>
              <Grid item>
                <Grid container justify="space-between">
                  <Grid item>
                    <Typography variant="h5">Confirmed:</Typography>
                  </Grid>
                  <Grid item>
                    <Typography variant="body1">
                      {carWashStatsResponse.cofirmedServices}
                    </Typography>
                  </Grid>
                </Grid>
              </Grid>
              <Grid item>
                <Grid container justify="space-between">
                  <Grid item>
                    <Typography variant="h5">To be charged:</Typography>
                  </Grid>
                  <Grid item>
                    <Typography variant="body1">
                      {carWashStatsResponse.toBeCharged}
                    </Typography>
                  </Grid>
                </Grid>
              </Grid>
              <Grid item>
                <Grid container justify="space-between">
                  <Grid item>
                    <Typography variant="h5">Profit per service:</Typography>
                  </Grid>
                  <Grid item>
                    <Typography variant="body1">
                      {carWashStatsResponse.averageProfitPerService}
                    </Typography>
                  </Grid>
                </Grid>
              </Grid>
              <Grid item>
                <Grid container justify="space-between">
                  <Grid item>
                    <Typography variant="h5">Profit:</Typography>
                  </Grid>
                  <Grid item>
                    <Typography variant="body1">
                      {carWashStatsResponse.profit}
                    </Typography>
                  </Grid>
                </Grid>
              </Grid>
            </Grid>
          )}
        </Grid>
      </DialogContent>
    </Dialog>
  );

  const serviceTypeAddDialog = (
    <Dialog
      open={serviceTypeDialogIsOpen}
      onClose={() => {
        setServiceTypeDialogIsOpen(false);
      }}
      TransitionComponent={Transition}
      fullScreen={matchesXS ? true : false}
      style={{ zIndex: matchesXS ? theme.zIndex.modal + 1 : undefined }}
    >
      <DialogContent
        dividers
        style={{ paddingBottom: "2rem", minWidth: "20rem" }}
      >
        <Grid container direction="column">
          <Grid item>
            <Typography variant="h2" align="center">
              Add new service type
            </Typography>
          </Grid>
          <Grid item>
            <TextField
              InputProps={{
                startAdornment: (
                  <InputAdornment position="start"><TextFieldsOutlinedIcon color='secondary'/></InputAdornment>
                ),
              }}
              inputRef={serviceTypeNameRef}
              id="service-type-name"
              label="Service Type Name"
              type='text'
              fullWidth
            />
            <TextField
              InputProps={{
                startAdornment: (
                  <InputAdornment position="start"><TimerOutlinedIcon color='secondary'/></InputAdornment>
                ),
              }}
              inputRef={durationRef}
              id="service-duration"
              label="Duration/hrs"
              type='number'
              fullWidth
            />
            <TextField
              InputProps={{
                startAdornment: (
                  <InputAdornment position="start"><MonetizationOnOutlinedIcon color='secondary'/></InputAdornment>
                ),
              }}
              inputRef={priceRef}
              id="service-price"
              label="Price"
              type='number' min='0' step='0.01'
              fullWidth
            />
          </Grid>
        </Grid>
      </DialogContent>
      <DialogActions>
        <Grid container direction='row' justify='space-between'>
          <Grid item>
            <IconButton onClick={() => {setServiceTypeDialogIsOpen(false);}}>
              <ArrowBackIosOutlinedIcon color='secondary'/>Go back
            </IconButton>
          </Grid>
          <Grid item>
            <IconButton onClick={addNewServiceTypeHandler}>
              <AddOutlinedIcon color='secondary'/>Add to shop
            </IconButton>
          </Grid>
        </Grid>
      </DialogActions>
    </Dialog>
  );

  const confirmDeleteDialog = (
    <Dialog
      open={confirmDialogIsOpen}
      onClose={() => {
        setConfirmDialogIsOpen(false);
      }}
      TransitionComponent={Transition}
      fullScreen={matchesXS ? true : false}
      style={{ zIndex: matchesXS ? theme.zIndex.modal + 1 : undefined }}
    >
      <DialogContent dividers>
        <Typography
          variant="h2"
          align="center"
          style={{ marginBottom: "2rem" }}
        >
          Confirm Action
        </Typography>
        <Typography variant="h4" align="center">
          Are You sure Yoy want to delete car wash{" "}
          <span style={{ color: theme.palette.primary.main }}>
            {detailedCarWash.carWashName}
          </span>
          ?
        </Typography>
        <Typography
          variant="body1"
          align="center"
          style={{ marginTop: "2rem", color: "red" }}
        >
          NOTE: This action can not be undone!
        </Typography>
      </DialogContent>
      <DialogActions>
        <Grid container justify="space-between">
          <Grid item>
            <Button
              className={classes.dialogButton}
              onClick={() => {
                setConfirmDialogIsOpen(false);
              }}
            >
              Go back
            </Button>
          </Grid>
          <Grid item>
            <Button
              className={classes.dialogButton}
              onClick={deleteCarWashHandler}
            >
              Yes I'm sure
            </Button>
          </Grid>
        </Grid>
      </DialogActions>
    </Dialog>
  );

  const revenueDialog = (
    <Dialog
      open={revenueDialogIsOpen}
      onClose={() => {
        setRevenueDialogIsOpen(false);
        setRevenueFilterValue("");
        setRevenueResponse(null);
      }}
      TransitionComponent={Transition}
      fullScreen={matchesXS ? true : false}
      style={{ zIndex: matchesXS ? theme.zIndex.modal + 1 : undefined }}
    >
      <DialogContent dividers style={{ paddingBottom: "2rem" }}>
        <Grid container direction="column">
          <Grid item>
            <Typography
              align="center"
              variant="h2"
              style={{ marginBottom: "2rem" }}
            >
              Revenue Overview{" "}
              <span style={{ color: theme.palette.primary.main }}>
                {detailedCarWash.carWashName}
              </span>
            </Typography>
          </Grid>
          <Grid item container direction="column" spacing={3}>
            <Grid item>
              <FormControl className={classes.formControl} fullWidth>
                <InputLabel id="revenue-filter-select-label">
                  Revenue Type
                </InputLabel>
                <Select
                  fullWidth
                  labelId="revenue-filter-select-label"
                  id="revenue-filter-select"
                  value={revenueFilterValue}
                  onChange={(event) => {
                    setRevenueFilterValue(event.target.value);
                  }}
                >
                  <MenuItem value={1}>Daily</MenuItem>
                  <MenuItem value={2}>Weekly</MenuItem>
                  <MenuItem value={3}>Monthly</MenuItem>
                </Select>
              </FormControl>
            </Grid>
            {revenueResponse !== null && (
              <Grid item>
                <Typography variant="h2" align="center">
                  {revenueResponse}
                </Typography>
              </Grid>
            )}
          </Grid>
        </Grid>
      </DialogContent>
      <DialogActions>
        <Grid container justify="space-between">
          <Grid item>
            <Button
              className={classes.dialogButton}
              onClick={() => {
                setRevenueDialogIsOpen(false);
                setRevenueFilterValue("");
                setRevenueResponse(null);
              }}
            >
              Cancel
            </Button>
          </Grid>
          <Grid item>
            <Button
              className={classes.dialogButton}
              onClick={getCarWashRevenueHandler}
            >
              {isSubmitting ? <CircularProgress /> : "Request Revenue"}
            </Button>
          </Grid>
        </Grid>
      </DialogActions>
    </Dialog>
  );

  const editDialog = (
    <Dialog
      open={editDialogIsOpen}
      onClose={() => {
        setEditDialogIsOpen(false);
      }}
      TransitionComponent={Transition}
      fullScreen={matchesXS ? true : false}
      style={{ zIndex: matchesXS ? theme.zIndex.modal + 1 : undefined }}
    >
      <DialogContent dividers>
        <Grid container direction="column">
          <Grid item>
            {edit ? (
              <Typography
                align="center"
                variant="h2"
                style={{ marginBottom: "2rem" }}
              >
                Edit Shop{" "}
                <span style={{ color: theme.palette.primary.main }}>
                  "{detailedCarWash.carWashName}"
                </span>
              </Typography>
            ) : (
              <Typography
                align="center"
                variant="h2"
                style={{ marginBottom: "2rem" }}
              >
                Create New Car Wash
              </Typography>
            )}
          </Grid>
          <Grid item container direction="column" spacing={3}>
            <Grid item>
              <TextField
                inputRef={carWashNameRef}
                required
                fullWidth
                id="car-wash-name"
                label="Car Wash Name"
                type="text"
                defaultValue={edit ? detailedCarWash.carWashName : ""}
              />
            </Grid>
            <Grid item>
              <Grid
                container
                justify="space-between"
                direction={matchesXS ? "column" : "row"}
                spacing={matchesXS ? 3 : undefined}
              >
                <Grid item lg={5} md={5} sm={5}>
                  <TextField
                    inputRef={openingTimeRef}
                    fullWidth={matchesXS ? true : undefined}
                    required
                    id="opening-time"
                    label="Opening Time/Hrs"
                    type="number"
                    defaultValue={edit ? detailedCarWash.openingTime : ""}
                  />
                </Grid>
                <Grid item lg={5} md={5} sm={5}>
                  <TextField
                    inputRef={closingTimeRef}
                    fullWidth={matchesXS ? true : undefined}
                    required
                    id="closing-time"
                    label="Closing Time/Hrs"
                    type="number"
                    defaultValue={edit ? detailedCarWash.closingTime : ""}
                  />
                </Grid>
              </Grid>
            </Grid>
          </Grid>
        </Grid>
      </DialogContent>
      <DialogActions>
        <Grid container justify="space-between">
          <Grid item>
            <Button
              className={classes.dialogButton}
              onClick={() => {
                setEditDialogIsOpen(false);
              }}
            >
              Cancel
            </Button>
          </Grid>
          <Grid item>
            <Button
              className={classes.dialogButton}
              onClick={editCarWashHandler}
            >
              {isSubmitting ? (
                <CircularProgress />
              ) : edit ? (
                "Submit Changes"
              ) : (
                "Create Shop"
              )}
            </Button>
          </Grid>
        </Grid>
      </DialogActions>
    </Dialog>
  );

  return (
    <Grid container direction="column">
      <Grid item container className={classes.topBlock} justify="space-between">
        <Grid item lg={5} md={12}>
          {/* text */}
          <Grid
            container
            direction="column"
            style={{
              height: "100%",
              backgroundColor: "#00000099",
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
                All of Your shops here!
              </Typography>
            </Grid>
            <Grid item>
              <Typography
                variant="h4"
                align="center"
                style={{ color: "white" }}
                paragraph
              >
                Start new business by creating a new car wash, or edit Your
                existing ones, easy.
              </Typography>
            </Grid>
            <Grid item>
              <Typography
                variant="body1"
                align="center"
                paragraph
                style={{ color: "white", textOverflow: "clip" }}
              >
                Manage your shops, create new, or close an existing one, add new
                types of services and more...
              </Typography>
            </Grid>
          </Grid>
        </Grid>
      </Grid>
      <Grid item container className={classes.carWashesList} justify="center">
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
              <ListItem
                key={index}
                divider
                disableGutters
                style={{ padding: "2rem 0" }}
              >
                <CarWashDetails
                  carwash={carwash}
                  setDetailedCarWash={setDetailedCarWash}
                  setEditDialogIsOpen={setEditDialogIsOpen}
                  setRevenueDialogIsOpen={setRevenueDialogIsOpen}
                  setConfirmDialogIsOpen={setConfirmDialogIsOpen}
                  setServiceTypeDialogIsOpen={setServiceTypeDialogIsOpen}
                  setCarWashInfoDialogIsOpen={setCarWashInfoDialogIsOpen}
                  getCarWashStats={getCarWashStatsHandler}
                  setEdit={setEdit}
                />
              </ListItem>
            ))}
          </List>
        )}
      </Grid>
      {!editDialogIsOpen && !confirmDialogIsOpen && !revenueDialogIsOpen && (
        <Zoom in>
          <Fab
            color="secondary"
            aria-label="add-car-wash"
            onClick={() => {
              setEdit(false);
              setEditDialogIsOpen(true);
            }}
            style={{
              position: "fixed",
              right: "10%",
              bottom: "10%",
              zIndex: theme.zIndex.modal,
            }}
          >
            <AddIcon style={{ color: "white" }} />
          </Fab>
        </Zoom>
      )}
      {snackBar}
      {editDialog}
      {revenueDialog}
      {confirmDeleteDialog}
      {serviceTypeAddDialog}
      {carWashInfoDialog}
    </Grid>
  );
};

export default MyShopsPage;
