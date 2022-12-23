import { useEffect } from "react";
import { useLocation } from "react-router-dom";

const ScrollToTop = (props) => {
  const location = useLocation();
  useEffect(() => {
    if (!location.hash) {
      window.scroll({top:0, left:0,behavior: 'smooth'});
    }
  }, [location]);

  return <>{props.children}</>
};

export default ScrollToTop;