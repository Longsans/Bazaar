import { Router } from "react-router-dom";
import Inventory from "./components/Inventory";

const AppRoutes = [
  {
    path: "/inventory",
    element: <Inventory />,
  },
];

export default AppRoutes;
