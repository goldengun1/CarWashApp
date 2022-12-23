import React, { useCallback, useEffect, useState } from "react";

const AuthContext = React.createContext({
  token: "",
  isLoggedIn: false,
  isOwner: false,
  login: (token, expirationTime,isOwner) => {},
  logout: () => {},
});

let tokenRenewTimer;
//returns in miliseconds
const calculateRemainingTime = (expirationTime) => {
  const currTime = new Date().getTime();
  const expiration = new Date(expirationTime).getTime();

  return expiration - currTime;
};

const retrieveStoredToken = () => {
  const storedToken = localStorage.getItem("token");
  const storedTokenExpirationDate = localStorage.getItem("expirationTime");

  const remaining = calculateRemainingTime(storedTokenExpirationDate);

  return { token: storedToken, duration: remaining };
};

export const AuthContextProvider = (props) => {
  const tokenData = retrieveStoredToken();
  let initialToken;
  if (tokenData) {
    initialToken = tokenData.token;
  }
  const [token, setToken] = useState(initialToken);
  const usrIsLoggedIn = !!token;
  const isOwner = localStorage.getItem("isOwner") === "true";

  const logoutHandler = useCallback(() => {
    setToken(null);
    clearTimeout(tokenRenewTimer);
    localStorage.removeItem("token");
    localStorage.removeItem("expirationTime");
    localStorage.removeItem("isOwner");
    fetch("https://localhost:7092/api/users/signout", {
      method: "POST",
      headers: {
        Authorization: `Bearer ${token}`,
      },
    })
      .then((response) => {
        if (response.ok) {
          return response.json();
        }
        else{
          return response.json().then((data) => {
            console.log(data);
            let errorMessage = "Action Failed";
            if (data && data.error) {
              errorMessage = data.error;
            }
            throw new Error(errorMessage);
          });
        }
      })
      .then((responseData) => {
        console.log(responseData);
      })
      .catch(err => {
        console.log(err);
      });
  }, [token]);

  const renewTokenHandler = useCallback(() => {
    fetch('https://localhost:7092/api/users/renewtoken', {
      method: "GET",
      headers: {
        Authorization: `Bearer ${token}`,
      },
    })
      .then((response) => {
        if (!response.ok) {
          setToken(null);
          localStorage.removeItem("token");
          localStorage.removeItem("expirationTime");
        } else {
          return response.json();
        }
      })
      .then((data) => {
        // console.log("TOKEN RENEWED");
        localStorage.setItem("token", data.token);
        localStorage.setItem("expirationTime", data.expiration);
        setToken(data.token);
      });
  }, [token]);

  const loginHandler = (tokenparam, expirationTime,isOwner) => {
    localStorage.setItem("token", tokenparam);
    localStorage.setItem("expirationTime", expirationTime);
    localStorage.setItem("isOwner",isOwner);
    setToken(tokenparam);
  };

  useEffect(() => {
    if (tokenData.token) {
      // console.log(tokenData);
      tokenRenewTimer = setTimeout(
        renewTokenHandler,
        tokenData.duration - 5000
      );
    }
    return () => {
      clearTimeout(tokenRenewTimer);
    };
  }, [tokenData, renewTokenHandler]);

  return (
    <AuthContext.Provider
      value={{
        token,
        isLoggedIn: usrIsLoggedIn,
        isOwner,
        login: loginHandler,
        logout: logoutHandler,
      }}
    >
      {props.children}
    </AuthContext.Provider>
  );
};

export default AuthContext;
