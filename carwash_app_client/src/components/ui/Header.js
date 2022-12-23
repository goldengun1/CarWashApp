import {
  AppBar,
  Button,
  IconButton,
  List,
  ListItem,
  ListItemText,
  Menu,
  MenuItem,
  SwipeableDrawer,
  Tab,
  Tabs,
  Toolbar,
  useMediaQuery,
  useScrollTrigger,
  useTheme,
} from "@material-ui/core";
import MenuIcon from "@material-ui/icons/Menu";
import React, {Fragment, useContext, useEffect, useState} from "react";
import { makeStyles } from "@material-ui/core/styles";
import { Link } from "react-router-dom";
import logo from "../../assets/images/logo.png";
import AuthContext from "../../context/auth-context";

const ElevationScroll = (props) => {
  const { children, window } = props;

  const trigger = useScrollTrigger({
    disableHysteresis: true,
    threshold: 0,
    target: window ? window() : undefined,
  });

  return React.cloneElement(children, {
    elevation: trigger ? 4 : 0,
  });
};

const useStyles = makeStyles((theme) => ({
  appBar: {
    zIndex: theme.zIndex.modal + 1,
    height: "8rem",
    [theme.breakpoints.down("md")]: {
      height: "6rem",
    },
    [theme.breakpoints.down("xs")]: {
      height: "4rem",
    },
  },
  toolbarMargin: {
    ...theme.mixins.toolbar,
    marginBottom: "4.5rem",
    [theme.breakpoints.down("md")]: {
      marginBottom: "1.5rem",
    },
    [theme.breakpoints.down("xs")]: {
      marginBottom: "0",
    },
  },
  logo: {
    height: "3rem",
    marginTop: "2rem",
    [theme.breakpoints.down("md")]: {
      height: "2.5rem",
      marginTop: "0.5rem",
    },
    [theme.breakpoints.down("xs")]: {
      height: "2rem",
      marginTop: "0",
    },
  },
  logoButton: {
    padding: 0,
    "&:hover": {
      background: "transparent",
    },
    marginLeft: "1rem",
  },
  tabContainer: {
    marginLeft: "auto",
    marginTop: "auto",
  },
  tab: {
    ...theme.typography.tab,
    color: "white",
    // minWidth: "200px",
    marginLeft: "25px",
  },
  menu: {
    backgroundColor: theme.palette.primary.main,
    color: "white",
    borderRadius: 0,
    width: "12rem",
  },
  menuItem: {
    ...theme.typography.tab,
    opacity: "0.7",
    "&:hover": {
      opacity: 1,
    },
    getContentAnchorEl: null,
  },
  logoutBtn: {
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
  drawer: {
    backgroundColor: theme.palette.primary.main,
    minWidth: "20rem",
    [theme.breakpoints.down("xs")]: {
      width: "100%",
    },
  },
  drawerItem: {
    ...theme.typography.tab,
    color: "white",
    opacity: "0.7",
    "&:hover": {
      opacity: 1,
      backgroundColor: "rgba(170,255,255,0.34)",
    },
    [theme.breakpoints.down("xs")]: {
      textAlign: "center",
    },
  },
  drawerItemSelected: {
    opacity: 1,
    backgroundColor: "#1f597e",
  },
  drawerIconContainer: {
    marginLeft: "auto",
    marginTop: "10px",
  },
  drawerIcon: {
    height: "50px",
    width: "50px",
    color: "#FFFFFFAA"
  },
  drawerLogoutBtn: {
    backgroundColor: theme.palette.common.sienna,
    color: "white",
    fontWeight: 700,
    fontFamily: "Segoe UI",
    borderRadius: "0px",
    "&:hover": {
      backgroundColor: "#a26a47",
    },
    [theme.breakpoints.down("xs")]: {
      textAlign: "center",
    },
  },
}));

const Header = (props) => {
  //STATES
  const authCtx = useContext(AuthContext);
  const [menuIsOpen, setMenuIsOpen] = useState(false);
  const [anchorEl, setAnchorEl] = useState(null);
  const [drawerIsOpen, setDrawerIsOpen] = useState(false);

  const classes = useStyles();
  const theme = useTheme();
  const { isOwner, tabsValue, setTabsValue, selectedIndex, setSelectedIndex } = props;
  const matchesMD = useMediaQuery(theme.breakpoints.down("md"));
  const iOS = window && /iPad|iPhone|iPod/.test(navigator.userAgent);

  //HANDLERS
  const changeNavTabsHandler = (event, value) => {
    setTabsValue(value);
  };

  const openMenuHandler = (event) => {
    setAnchorEl(event.currentTarget);
    setMenuIsOpen(true);
  };

  const closeMenuHandler = (event) => {
    setAnchorEl(null);
    setMenuIsOpen(false);
  };

  const menuItemClickHandler = (event, index) => {
    setAnchorEl(null);
    setMenuIsOpen(false);
    setSelectedIndex(index);
    setTabsValue(1);
  };

  const logoutHandler = (event) => {
    authCtx.logout();
  };

  //VARS
  const menuOptions = [
    { name: "Studies", route: "/studies" },
    { name: "Exams", route: "/exams" },
    { name: "Courses", route: "/courses" },
  ];

  const drawerOptions = [
    { name: "Home", route: "/" },
    { name: isOwner ? "My Shops" : "My Reservations", route: isOwner ? "/myshops" : "/myreservations" },
    { name: "Profile", route: "/profile" },
    { name: "Support", route: "/support" },
  ];

  useEffect(() => {
    const path = window.location.pathname;
    switch (path) {
      case "/":
        setTabsValue(0);
        break;
      case "/myshops":
        setTabsValue(1);
        break;
      case "/myreservations":
        setTabsValue(1);
        break;
      case "/profile":
        setTabsValue(2);
        break;
      case "/support":
        setTabsValue(3);
        break;
      default:
        console.log(`ERROR PATH: ${path}`);
        break;
    }
  }, [setSelectedIndex, setTabsValue]);

  //JSX ELEMENTS
  const tabs = (
    <Fragment>
      <Tabs
        value={tabsValue}
        onChange={changeNavTabsHandler}
        className={classes.tabContainer}
      >
        <Tab className={classes.tab} component={Link} to="/" label="Home" />
        <Tab
          // aria-owns={anchorEl ? "studies-menu" : undefined}
          // aria-haspopup={anchorEl ? "true" : undefined}
          // onMouseOver={openMenuHandler}
          className={classes.tab}
          component={Link}
          to={isOwner ? "/myshops" : "/myreservations"}
          label={isOwner ? "My Shops" : "My Reservations"}
        />
        <Tab
          className={classes.tab}
          component={Link}
          to="/profile"
          label="Profile"
        />
        <Tab
          className={classes.tab}
          component={Link}
          to="/support"
          label="Support"
        />
      </Tabs>
      <Button className={classes.logoutBtn} onClick={logoutHandler}>
        Log Out
      </Button>
      <Menu
        id="studies-menu"
        open={menuIsOpen}
        onClose={closeMenuHandler}
        anchorEl={anchorEl}
        elevation={0}
        keepMounted
        MenuListProps={{ onMouseLeave: closeMenuHandler }}
        getContentAnchorEl={null}
        classes={{ paper: classes.menu }}
        style={{ zIndex: 1500 }}
      >
        {menuOptions.map((menuItem, index) => (
          <MenuItem
            key={index}
            component={Link}
            to={menuItem.route}
            className={classes.menuItem}
            onClick={(event) => {
              menuItemClickHandler(event, index);
            }}
            selected={index === selectedIndex && tabsValue === 1}
          >
            {menuItem.name}
          </MenuItem>
        ))}
      </Menu>
    </Fragment>
  );

  const drawer = (
    <Fragment>
      <SwipeableDrawer
        onClose={() => {
          setDrawerIsOpen(false);
        }}
        onOpen={() => {
          setDrawerIsOpen(true);
        }}
        open={drawerIsOpen}
        classes={{ paper: classes.drawer }}
        disableBackdropTransition={!iOS}
        disableDiscovery={iOS}
      >
        <div className={classes.toolbarMargin} />
        <List>
          {drawerOptions.map((drawerItem, index) => (
            <ListItem
              key={index}
              button
              divider
              className={tabsValue === index ? [classes.drawerItem,classes.drawerItemSelected] : classes.drawerItem}
              onClick={() => {
                setDrawerIsOpen(false);
                setTabsValue(index);
              }}
              component={Link}
              to={drawerItem.route}
            >
              <ListItemText>{drawerItem.name}</ListItemText>
            </ListItem>
          ))}
          <ListItem
            button
            divider
            className={classes.drawerLogoutBtn}
            onClick={() => {
              authCtx.logout();
            }}
            component={Button}
          >
            <ListItemText>Log Out</ListItemText>
          </ListItem>
        </List>
      </SwipeableDrawer>
      <IconButton
        onClick={() => setDrawerIsOpen((prev) => !prev)}
        className={classes.drawerIconContainer}
      >
        <MenuIcon className={classes.drawerIcon}/>
      </IconButton>
    </Fragment>
  );

  return (
    <Fragment>
      <ElevationScroll>
        <AppBar position="fixed" className={classes.appBar}>
          <Toolbar disableGutters>
            <Button
              disableRipple
              component={Link}
              to="/"
              className={classes.logoButton}
              onClick={() => {
                setTabsValue(0);
              }}
            >
              <img src={logo} alt="site logo" className={classes.logo} />
            </Button>
            {matchesMD ? drawer : tabs}
          </Toolbar>
        </AppBar>
      </ElevationScroll>
      <div className={classes.toolbarMargin} />
    </Fragment>
  );
};

export default Header;
