import {Grid, Hidden} from "@material-ui/core";
import globe from "../../assets/icons/globe.svg";
import facebook from "../../assets/icons/facebook.svg";
import instagram from "../../assets/icons/instagram.svg";
import github from "../../assets/icons/github.png";
import gitlab from "../../assets/icons/gitlab.png";
import footerImg from "../../assets/images/Footer Adornment.svg";
import {makeStyles} from "@material-ui/core/styles";
import {Link} from "react-router-dom";

const useStyles = makeStyles((theme) => ({
  footer: {
    width: "100%",
    position: "relative",
    backgroundColor: theme.palette.primary.main,
    alignContent: 'center'
  },
  mainContainer: {
    position: "absolute",
    display: "flex",
    // height: "100%",
    marginTop: "4rem",
  },
  gridItem: {
    margin: "3rem",
  },
  footerImg: {
    height: "15rem",
    verticalAlign: "bottom",
    [theme.breakpoints.down("md")]: {
      height: "12rem",
    },
    [theme.breakpoints.down("xs")]: {
      height: "10rem",
    },
  },
  link: {
    color: "#FFFFFF",
    textDecoration: "none",
    fontSize: "0.75em",
    fontWeight: "bold",
    fontFamily: "Segoe UI",
  },
  socialContainer: {
    position: "absolute",
    marginTop: "-6rem",
    right: "3rem",
    width: 'auto'
  },
  icon: {
    height: "4rem",
    width: "4rem",
    [theme.breakpoints.down('md')] : {
      marginTop: "-4rem",
    },
    [theme.breakpoints.down('xs')] : {
      height: "3rem",
      width: "3rem",
      marginTop: "-4rem",
    }
  },
}));

const Footer = (props) => {
  const classes = useStyles();
  const {isOwner, setTabsValue} = props;
  return (
    <footer className={classes.footer}>
      <Hidden mdDown>
        <Grid container justify="center" className={classes.mainContainer}>
          <Grid item className={classes.gridItem}>
            <Grid container direction="column">
              <Grid
                item
                component={Link}
                className={classes.link}
                to="/"
                onClick={() => {
                  setTabsValue(0);
                }}
              >
                Home
              </Grid>
            </Grid>
          </Grid>
          <Grid item className={classes.gridItem}>
            <Grid container spacing={3} direction="column">
              <Grid
                item
                component={Link}
                className={classes.link}
                to={isOwner ? "/myshops" : "/myreservations"}
                onClick={() => {
                  setTabsValue(1);
                  
                }}
              >
                {isOwner ? "My Shops" : "My Reservations"}
              </Grid>
            </Grid>
          </Grid>
          <Grid item className={classes.gridItem}>
            <Grid container direction="column">
              <Grid
                item
                component={Link}
                className={classes.link}
                to="/profile"
                onClick={() => {
                  setTabsValue(2);
                }}
              >
                Profile
              </Grid>
            </Grid>
          </Grid>
          <Grid item className={classes.gridItem}>
            <Grid container direction="column">
              <Grid
                item
                component={Link}
                className={classes.link}
                to="/support"
                onClick={() => {
                  setTabsValue(3);
                }}
              >
                Support
              </Grid>
            </Grid>
          </Grid>
          <Grid item className={classes.gridItem}>
            <Grid container direction="column">
              <Grid
                item
                component={"a"}
                className={classes.link}
                href="https://github.com/goldengun1"
                rel="noopener noreferrer"
                target="blank"
              >
                Development & More
              </Grid>
            </Grid>
          </Grid>
        </Grid>
      </Hidden>
      <img
        alt="footer decoration"
        src={footerImg}
        className={classes.footerImg}
      />
      <Grid container justify='flex-end' spacing={3} className={classes.socialContainer}>
        <Grid
          item
          component={"a"}
          href="http://www.matf.bg.ac.rs/"
          rel="noopener noreferrer"
          target="blank"
        >
          <img alt="web logo" src={globe} className={classes.icon} />
        </Grid>
        <Grid
          item
          component={"a"}
          href="https://github.com/goldengun1"
          rel="noopener noreferrer"
          target="blank"
        >
          <img alt="facebook logo" src={github} className={classes.icon} />
        </Grid>
        <Grid
          item
          component={"a"}
          href="http://10.35.101.179/mentori/mihailo-simic-carwash-app"
          rel="noopener noreferrer"
          target="blank"
        >
          <img alt="instagram" src={gitlab} className={classes.icon} />
        </Grid>
      </Grid>
    </footer>
  );
};

export default Footer;