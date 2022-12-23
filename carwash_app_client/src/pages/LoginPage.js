import { makeStyles } from "@material-ui/core/styles";
import React, { Fragment, useContext, useRef, useState } from "react";
import {
  Button,
  Checkbox,
  CircularProgress,
  FormControlLabel,
  Grid,
  Paper, Slide, Snackbar,
  TextField,
  Typography,
} from "@material-ui/core";
import theme from "../components/ui/Theme";
import logo from "../assets/images/logo.png";
import { useHistory } from "react-router-dom";
import AuthContext from "../context/auth-context";

const useStyles = makeStyles((theme) => ({
  backdrop: {
    background: `linear-gradient(to bottom,${theme.palette.common.blue} 0%,${theme.palette.common.blue} 35%, #FFFFFF 30%)`,
    paddingTop: "3rem",
    backgroundSize: "cover",
    backgroundPosition: "bottom",
    width: "100%",
    minHeight: "50rem",
    zIndex: "-10",
  },
  form: {
    backgroundColor: "white",
    maxWidth: "30rem",
    marginLeft: "auto",
    marginRight: "auto",
    marginTop: "10rem",
    borderRadius: "5px",
    [theme.breakpoints.down("md")]: {
      borderRadius: "0px",
    },
  },
  submit: {
    width: "15rem",
    height: "2rem",
    backgroundColor: theme.palette.common.blue,
    borderRadius: "25px",
    fontFamily: "Arial",
    padding: "25px",
    border: `1px solid ${theme.palette.common.blue}`,
    color: "white",
    "&:hover": {
      color: theme.palette.common.blue,
    },
  },
  logo: {
    backgroundImage: `url(${logo})`,
    backgroundSize: "contain",
    backgroundRepeat: "no-repeat",
    width: "13rem",
    height: "4rem",
    left: "10rem",
    top: "5rem",
    // position: "absolute",
    position: "inherit",
    marginLeft: "auto",
    marginRight: "auto",
    marginTop: "3rem",
    marginBottom: "-5rem",
  },
  snackBarErr: {
    backgroundColor: "#f44336",
    color: "white",
    fontFamily: "Segoe UI",
    textAlign: "center",
    fontSize: "1rem",
    fontWeight: "bold",
  }
}));

function SnackBarTransition(props) {
  return <Slide {...props} direction="down" />;
}

const LoginPage = () => {
  const classes = useStyles();
  const [login, setLogin] = useState(true);
  const [isLoading, setIsLoading] = useState(false);
  const [isOwnerChecked, setIsOwnerChecked] = useState(false);
  const [snackBarShown, setSnackBarShown] = useState(false);
  const [snackBarMessage, setSnackBarMessage] = useState("");

  const history = useHistory();
  const authContext = useContext(AuthContext);
  const usernameRef = useRef();
  const emailRef = useRef();
  const passwordRef = useRef();
  const firstNameRef = useRef();
  const lastNameRef = useRef();

  //HANDLERS
  const handleClose = (event, reason) => {
    if (reason === "clickaway") {
      return;
    }

    setSnackBarShown(false);
  };

  const onSubmitFormHandler = (event) => {
    event.preventDefault();
    setIsLoading(true);

    const Username = usernameRef.current.value;
    const Password = passwordRef.current.value;
    let url;
    let userInfoDTO = { userName: Username, password: Password };

    if (login) {
      url = "https://localhost:7092/api/users/signin";
    } else {
      url = "https://localhost:7092/api/users/signup";
      userInfoDTO = {
        ...userInfoDTO,
        email: emailRef.current.value,
        firstName: firstNameRef.current.value,
        lastName: lastNameRef.current.value,
        isOwner: isOwnerChecked,
      };
    }
    console.log(userInfoDTO);

    fetch(url, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({ ...userInfoDTO }),
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
        authContext.login(responseData.token, responseData.expiration,login ? responseData.isOwner : isOwnerChecked);
        history.replace("/");
        setIsLoading(false);
      })
      .catch((err) => {
        console.log(err.message);
        setIsLoading(false);
        setSnackBarShown(true);
        setSnackBarMessage(`${err.message}`);
      });
  };

  //JSX ELEMENTS
  const snackBar = (
    <Snackbar
      open={snackBarShown}
      message={snackBarMessage}
      ContentProps={{
        classes: {
          root: classes.snackBarErr,
        },
      }}
      anchorOrigin={{ vertical: "top", horizontal: "center" }}
      autoHideDuration={3000}
      TransitionComponent={SnackBarTransition}
      onClose={handleClose}
    />);

  const registrationFormAddOn = (
    <>
      <Grid item>
        <TextField
          id="firstname"
          label="First Name"
          type="text"
          required
          fullWidth
          inputProps={{ ref: firstNameRef }}
        />
      </Grid>
      <Grid item>
        <TextField
          id="lastname"
          label="Last Name"
          type="text"
          required
          fullWidth
          inputProps={{ ref: lastNameRef }}
        />
      </Grid>
      <Grid item>
        <FormControlLabel style={{marginTop: "0.5rem"}}
          control={
            <Checkbox
              checked={isOwnerChecked}
              name="isOwner"
              onChange={() => {
                setIsOwnerChecked((prev) => !prev);
              }}
            />
          }
          label="Owner"
        />
      </Grid>
      <Grid item>
        <TextField
          id="email"
          label="Email"
          type="email"
          required
          fullWidth
          inputProps={{ ref: emailRef }}
        />
      </Grid>
    </>
  );

  return (
    <Fragment>
      <div className={classes.backdrop}>
        <div className={classes.logo}>{/*<img src={logo}/>*/}</div>
        <Paper classes={{ root: classes.form }}>
          <form onSubmit={onSubmitFormHandler}>
            <Grid
              container
              justify="center"
              alignItems="center"
              direction="column"
              style={{ paddingTop: "50px", padding: "30px 50px 15px 50px" }}
            >
              <Grid item>
                <Typography align="center" variant="h4">
                  {login ? "Log In" : "Sign Up"}
                </Typography>
              </Grid>
              <Grid item container direction="column" spacing={1}>
                {!login && registrationFormAddOn}
                <Grid item>
                  <TextField
                    id="username"
                    label="Username"
                    required
                    fullWidth
                    inputProps={{ ref: usernameRef }}
                  />
                </Grid>
                <Grid item>
                  <TextField
                    id="password"
                    label="Password"
                    type="password"
                    required
                    fullWidth
                    inputProps={{ ref: passwordRef }}
                  />
                </Grid>
              </Grid>
              <Grid
                item
                container
                justify="center"
                style={{ marginTop: "1.5rem" }}
              >
                {isLoading ? (
                  <CircularProgress color="secondary" />
                ) : (
                  <Button
                    className={classes.submit}
                    type="submit"
                    onClick={() => {}}
                  >
                    {login ? "Log In" : "Sign Up"}
                  </Button>
                )}
              </Grid>
              <Grid
                item
                justify="space-between"
                container
                direction="row"
                style={{ marginTop: "2rem" }}
              >
                <Typography variant="body2">
                  {login ? "Don't have an account?" : "Alredy have an account?"}
                </Typography>
                <Button
                  disableRipple
                  disableFocusRipple
                  style={{ color: theme.palette.common.blue, lineHeight: 0 }}
                  onClick={(event) => {
                    setLogin((prev) => !prev);
                  }}
                >
                  {login ? "Register" : "Log In"}
                </Button>
              </Grid>
            </Grid>
          </form>
        </Paper>
      </div>
      {snackBar}
    </Fragment>
  );
};

export default LoginPage;
