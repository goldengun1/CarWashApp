import { ThemeProvider } from "@material-ui/styles";
import theme from "./components/ui/Theme";
import React from "react";
import { BrowserRouter, Redirect, Route, Switch } from "react-router-dom";
import ScrollToTop from "./components/ui/ScrollToTop";
import LoginPage from "./pages/LoginPage";
import HomePage from "./pages/HomePage";
import { useContext, useState } from "react";
import AuthContext from "./context/auth-context";
import Header from "./components/ui/Header";
import Footer from "./components/ui/Footer";
import ProfilePage from "./pages/ProfilePage";
import SupportPage from "./pages/SupportPage";
import MyReservationsPage from "./pages/MyReservationsPage";
import MyShopsPage from "./pages/MyShopsPage";

function App() {
  const [tabsValue, setTabsValue] = useState(0);
  const [selectedIndex, setSelectedIndex] = useState(0);
  const authContext = useContext(AuthContext);
  const { isLoggedIn, isOwner } = authContext;

  return (
    <ThemeProvider theme={theme}>
      <BrowserRouter>
        <ScrollToTop />
        {isLoggedIn && (
          <Header
            isOwner={isOwner}
            tabsValue={tabsValue}
            setTabsValue={setTabsValue}
            selectedIndex={selectedIndex}
            setSelectedIndex={setSelectedIndex}
          />
        )}
        <Switch>
          <Route exact path="/">
            {isLoggedIn ? (
              <HomePage
                setTabsValue={setTabsValue}
                setSelectedIndex={setSelectedIndex}
              />
            ) : (
              <Redirect to="/login" />
            )}
          </Route>
          <Route path="/login">
            {!isLoggedIn ? <LoginPage /> : <Redirect to="/" />}
          </Route>
          <Route path="/myreservations">
            {/*{(isLoggedIn && !isOwner) ? <MyReservationsPage/> : <Redirect to="/myshops"/>}*/}
            {isLoggedIn ? !isOwner ? <MyReservationsPage/> : <Redirect to="/myshops" /> : <Redirect to="/login" />}
          </Route>
          <Route path="/myshops">
            {/*{(isLoggedIn && isOwner) ? <MyShopsPage/> : <Redirect to="/myreservations"/>}*/}
            {isLoggedIn ? isOwner ? <MyShopsPage/> : <Redirect to="/myreservations" /> : <Redirect to="/login" />}
          </Route>
          <Route path="/profile">
            {isLoggedIn ? (
              <ProfilePage/>
            ) : (
              <Redirect to="/login" />
            )}
          </Route>
          <Route path="/support">
            {isLoggedIn ? (
              <SupportPage/>
            ) : (
              <Redirect to="/login" />
            )}
          </Route>
          <Route path="*">
            {isLoggedIn ? (
              <Redirect to="/"/>
            ) : (
              <Redirect to="/login" />
            )}
          </Route>
        </Switch>
        <Footer isOwner={isOwner} setTabsValue={setTabsValue} />
      </BrowserRouter>
    </ThemeProvider>
  );
}

export default App;
