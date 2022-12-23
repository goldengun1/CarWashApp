import {
  Avatar,
  Button,
  CircularProgress,
  Dialog,
  DialogActions,
  DialogContent,
  Divider,
  Grid,
  IconButton,
  LinearProgress,
  Paper,
  Slide,
  Snackbar,
  TextField,
  Typography,
  useMediaQuery,
  Zoom,
} from "@material-ui/core";
import { makeStyles } from "@material-ui/core/styles";
import profileMainBackground from "../assets/images/material-design-blue-and-white.jpg";
import profileMainBackgroundMobile from "../assets/images/material-design-blue-and-white-mobile.jpg";
import profileCardBackground from "../assets/images/profie_card_background_material.jpg";
import VerifiedUserIcon from "@material-ui/icons/VerifiedUser";
import CancelIcon from "@material-ui/icons/Cancel";
import theme from "../components/ui/Theme";
import { PhotoCamera } from "@material-ui/icons";
import React, { useContext, useEffect, useRef, useState } from "react";
import AuthContext from "../context/auth-context";
import Fab from "@material-ui/core/Fab";
import EditIcon from "@material-ui/icons/Edit";
import ClearOutlinedIcon from "@material-ui/icons/ClearOutlined";
import CheckOutlinedIcon from "@material-ui/icons/CheckOutlined";
import DeleteIcon from "@material-ui/icons/Delete";

const useStyles = makeStyles((theme) => ({
  profileMainBackground: {
    backgroundImage: `url(${profileMainBackground})`,
    backgroundSize: "cover",
    width: "100%",
    height: "100%",
    marginTop: "-0.5rem",
    [theme.breakpoints.down("xs")]: {
      backgroundImage: `url(${profileMainBackgroundMobile})`,
      backgroundSize: "cover",
      filter: "0.5",
    },
  },
  deleteBtn: {
    backgroundColor: theme.palette.secondary.main,
    width: "12rem",
    height: "3rem",
    color: "white",
    fontFamily: "Segoe UI",
    borderRadius: "10px",
    padding: "10px",
    border: `1px solid ${theme.palette.common.sienna}`,
    "&:hover": {
      backgroundColor: "#a26a47",
    },
  },
  mainContainer: {
    minHeight: "20rem",
    backgroundImage: `url(${profileCardBackground})`,
    backgroundSize: "contain",
    backgroundRepeat: "repeat",
    padding: "25px 0px",
    marginTop: "1rem",
  },
  avatar: {
    width: "8rem",
    height: "8rem",
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

const ProfilePage = (props) => {
  //STATES
  const [userProfile, setUserProfile] = useState({});
  const [isLoading, setisLoading] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [editDialogIsOpen, setEditDialogIsOpen] = useState(false);
  const [snackBarShown, setSnackBarShown] = useState(false);
  const [snackBarMessage, setSnackBarMessage] = useState({
    severity: "Info",
    message: "",
  });
  const firstNameRef = useRef();
  const lastNameRef = useRef();

  const classes = useStyles();
  const authCtx = useContext(AuthContext);
  const { token, logout } = authCtx;
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

  const editUserHandler = () => {
    setIsSubmitting(true);

    const userEditDTO = {
      userName: userProfile.userName,
      email: userProfile.email,
      firstName: firstNameRef.current.value,
      lastName: lastNameRef.current.value,
    };
    fetch("https://localhost:7092/api/users/edit", {
      method: "PUT",
      body: JSON.stringify(userEditDTO),
      headers: {
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json",
      },
    })
      .then((response) => {
        if (response.ok) {
          const profile = {
            ...userProfile,
            firstName: userEditDTO.firstName,
            lastName: userEditDTO.lastName,
          };
          setUserProfile(profile);
          setIsSubmitting(false);
          setEditDialogIsOpen(false);
          setSnackBarShown(true);
          setSnackBarMessage({severity: "Success", message: "Profile edited successfully."});
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
        setSnackBarShown(true);
        setSnackBarMessage({severity:"Error", message: `${err.message}`});
      });
  };
  const deleteUserHandler = () => {
    setIsSubmitting(true);
    fetch("https://localhost:7092/api/users/deleteaccount", {
      method: "DELETE",
      headers: {
        Authorization: `Bearer ${token}`,
      },
    })
      .then((response) => {
        if (response.ok) {
          setIsSubmitting(false);
          logout();
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
      });
  };

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

  const editProfileDialog = (
    <Dialog
      open={editDialogIsOpen}
      onClose={() => {
        setEditDialogIsOpen(false);
      }}
      TransitionComponent={Transition}
      fullScreen={matchesXS ? true : false}
      style={{ zIndex: matchesXS ? theme.zIndex.modal + 1 : undefined }}
    >
      <DialogContent
        dividers
        style={{ paddingBottom: "2rem", minWidth: "20rem" }}
      >
        <Grid container direction="column" spacing={3}>
          <Grid item>
            <Typography
              align="center"
              variant="h2"
              style={{ marginBottom: "2rem", color: "#555555" }}
            >
              Edit Profile
            </Typography>
          </Grid>
          <Grid item>
            <Grid
              container
              direction={matchesXS ? "column" : "row"}
              spacing={3}
            >
              <Grid item>
                <TextField
                  inputRef={firstNameRef}
                  fullWidth={matchesXS ? true : false}
                  label="First Name"
                  type="text"
                />
              </Grid>
              <Grid item>
                <TextField
                  inputRef={lastNameRef}
                  fullWidth={matchesXS ? true : false}
                  label="Last Name"
                  type="text"
                />
              </Grid>
            </Grid>
          </Grid>
          <Grid item>
            <TextField
              fullWidth
              label="Username"
              type="text"
              value={userProfile.userName}
              disabled
            />
          </Grid>
          <Grid item>
            <TextField
              fullWidth
              label="Email"
              type="email"
              value={userProfile.userName}
              disabled
            />
          </Grid>
        </Grid>
      </DialogContent>
      <DialogActions>
        <Grid container justifyContent="space-between">
          <Grid item>
            <IconButton
              onClick={() => {
                setEditDialogIsOpen(false);
              }}
            >
              <ClearOutlinedIcon style={{ color: "#b71515" }} />
              Reject
            </IconButton>
          </Grid>
          <Grid item>
            {isSubmitting ? (
              <CircularProgress />
            ) : (
              <IconButton onClick={editUserHandler}>
                <CheckOutlinedIcon style={{ color: "#009d19" }} />
                Confirm
              </IconButton>
            )}
          </Grid>
        </Grid>
      </DialogActions>
    </Dialog>
  );

  //FETCHING PROFILE ON LOAD
  useEffect(() => {
    setisLoading(true);
    fetch("https://localhost:7092/api/users/personal", {
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
        console.log(responseData);
        setUserProfile(responseData);
        setisLoading(false);
      })
      .catch((err) => {
        setisLoading(false);
        console.log();
      });
  }, []);

  return (
    <Grid
      container
      style={{ minHeight: "50rem" }}
      className={classes.profileMainBackground}
      direction="column"
      alignItems="center"
    >
      <Grid item style={{ width: matchesXS ? "100%" : "30rem" }}>
        <Paper classes={{ root: classes.mainContainer }}>
          {/*TODO:add skeleton when loading*/}
          {isLoading ? (
            <>
              <LinearProgress color="secondary" />
            </>
          ) : (
            <>
              <Grid
                container
                direction="column"
                justifyContent="center"
                alignItems="center"
              >
                <Grid
                  item
                  container
                  justifyContent="flex-end"
                  style={{ marginBottom: "1rem" }}
                >
                  <Grid item>
                    <Typography
                      variant="body1"
                      style={{
                        color: "white",
                        marginRight: "1rem",
                        textShadow: "0px 0px 10px #ffffff",
                      }}
                    >
                      {userProfile.accountType + " Account"}
                    </Typography>
                  </Grid>
                </Grid>
                <Grid item style={{ marginBottom: "0rem" }}>
                  <Avatar
                    alt="profile picture"
                    // src={personalImage}
                    className={classes.avatar}
                  />
                </Grid>
                <Grid item style={{ marginBottom: "1rem" }}>
                  <input
                    accept="image/*"
                    style={{ display: "none" }}
                    id="contained-button-file"
                    multiple
                    type="file"
                  />
                  <label htmlFor="contained-button-file">
                    <IconButton
                      style={{ color: "white" }}
                      aria-label="upload picture"
                      component="span"
                    >
                      <PhotoCamera />
                    </IconButton>
                  </label>
                </Grid>
              </Grid>
              <Grid item>
                <Grid
                  container
                  direction="column"
                  // spacing={3}
                  style={{ padding: "0 20px" }}
                >
                  <Grid item style={{ marginBottom: "1rem" }}>
                    <Grid container justifyContent="space-between">
                      <Grid item>
                        <Typography style={{ color: "white" }} variant="body1">
                          First Name
                        </Typography>
                      </Grid>
                      <Grid item style={{ marginBottom: "1rem" }}>
                        <Typography variant="h5" style={{ color: "white" }}>
                          {userProfile.firstName}
                        </Typography>
                      </Grid>
                    </Grid>
                  </Grid>
                  <Grid item style={{ marginBottom: "1rem" }}>
                    <Grid container justifyContent="space-between">
                      <Grid item>
                        <Typography style={{ color: "white" }} variant="body1">
                          Last Name
                        </Typography>
                      </Grid>
                      <Grid item>
                        <Typography variant="h5" style={{ color: "white" }}>
                          {userProfile.lastName}
                        </Typography>
                      </Grid>
                    </Grid>
                  </Grid>
                  <Grid item style={{ marginBottom: "1rem" }}>
                    <Grid container justifyContent="space-between">
                      <Grid item>
                        <Typography style={{ color: "white" }} variant="body1">
                          Username
                        </Typography>
                      </Grid>
                      <Grid item>
                        <Typography variant="h6" style={{ color: "white" }}>
                          {userProfile.userName}
                        </Typography>
                      </Grid>
                    </Grid>
                  </Grid>
                  <Grid item style={{ marginBottom: "1rem" }}>
                    <Grid container justifyContent="space-between">
                      <Grid item>
                        <Typography style={{ color: "white" }} variant="body1">
                          Email
                        </Typography>
                      </Grid>
                      <Grid item>
                        <Typography variant="h6" style={{ color: "white" }}>
                          {userProfile.email}
                        </Typography>
                      </Grid>
                    </Grid>
                  </Grid>
                  <Grid item style={{ marginBottom: "1rem" }}>
                    <Grid container justifyContent="space-between">
                      <Grid item>
                        <Typography style={{ color: "white" }} variant="body1">
                          Email Verified
                        </Typography>
                      </Grid>
                      <Grid item>
                        <Typography variant="h6" style={{ color: "white" }}>
                          {userProfile.EmailConfirmed ? (
                            <VerifiedUserIcon style={{ color: "white" }} />
                          ) : (
                            <CancelIcon style={{ color: "white" }} />
                          )}
                        </Typography>
                      </Grid>
                    </Grid>
                  </Grid>
                  <Divider />
                  <Grid item style={{ marginTop: "1.5rem" }}>
                    <Grid container justifyContent="center" alignItems="center">
                      <Grid item>
                        <Button
                          onClick={deleteUserHandler}
                          className={classes.deleteBtn}
                          startIcon={
                            !isSubmitting && (
                              <DeleteIcon style={{ color: "white" }} />
                            )
                          }
                        >
                          {isSubmitting ? (
                            <CircularProgress />
                          ) : (
                            "Close Account"
                          )}
                        </Button>
                      </Grid>
                    </Grid>
                  </Grid>
                </Grid>
              </Grid>
            </>
          )}
        </Paper>
      </Grid>
      {!editDialogIsOpen && (
        <Zoom in>
          <Fab
            color="secondary"
            aria-label="edit-profile"
            onClick={() => {
              setEditDialogIsOpen(true);
            }}
            style={{
              position: "fixed",
              right: "10%",
              bottom: "10%",
              zIndex: theme.zIndex.modal,
            }}
          >
            <EditIcon style={{ color: "white" }} />
          </Fab>
        </Zoom>
      )}
      {editProfileDialog}
      {snackBar}
    </Grid>
  );
};

export default ProfilePage;
