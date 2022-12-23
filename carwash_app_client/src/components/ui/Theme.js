import {createTheme} from "@material-ui/core/styles";

const blue = "#2773a1";
const sienna = "#D88C5E";
const oruby = "#A63446";
const comp = "#59CBB9";
const viridian = "#498467";
const mauve = "#B67B98"

export default createTheme({
  palette:{
    common:{
      blue,
      sienna,
      oruby,
      comp,
      viridian,
      mauve,
    },
    primary:{
      main: blue,
    },
    secondary:{
      main: sienna,
    },
  },
  typography:{
    tab:{
      textTransform: "none",
      fontWeight: 700,
      fontSize: "1rem",
      color: 'white',
      fontFamily: "Segoe UI"
    },
    body1:{
      fontFamily: "Segoe UI",
      fontSize: "1.2rem",
    },
    body2:{
      fontFamily: "Segoe UI",
    },
    h2: {
      fontFamily: "Segoe UI",
      fontWeight: 700,
      fontSize: "2.5rem",
      lineHeight: 1.5
    },
    h3: {
      fontFamily: "Segoe UI",
      fontSize: "2.5rem",
    },
    h4: {
      fontFamily: "Segoe UI",
      fontSize: "1.75rem",
      fontWeight: 500
    },
  },
  overrides:{

  },
});