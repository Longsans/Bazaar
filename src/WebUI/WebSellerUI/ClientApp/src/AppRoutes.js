import { Catalog } from "./components/Catalog";
import { Home } from "./components/Home";
import { Login } from "./components/Login";
import { Logout } from "./components/Logout";
import { Orders } from "./components/Orders";
import { Contracts } from "./components/Contracts";

const AppRoutes = [
  {
    index: true,
    element: <Home />,
  },
  {
    path: "/catalog",
    element: <Catalog />,
  },
  {
    path: "/orders",
    element: <Orders />,
  },
  {
    path: "/contracts",
    element: <Contracts />,
  },
  {
    path: "/bff/login",
    element: <Login />,
  },
  {
    path: "/bff/logout",
    element: <Logout />,
  },
];

export default AppRoutes;
