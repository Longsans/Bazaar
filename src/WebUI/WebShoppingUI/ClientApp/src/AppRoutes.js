import Basket from "./components/Basket";
import Catalog from "./components/Catalog";
import Checkout from "./components/Checkout";
import { Home } from "./components/Home";
import { Login } from "./components/Login";
import { Logout } from "./components/Logout";
import OrderHistory from "./components/OrderHistory";
import { Profile } from "./components/Profile";

const AppRoutes = [
  {
    index: true,
    element: <Home />,
  },
  {
    path: "/bff/login",
    element: <Login />,
  },
  {
    path: "/bff/logout",
    element: <Logout />,
  },
  {
    path: "/profile",
    element: <Profile />,
  },
  {
    path: "/catalog",
    element: <Catalog />,
  },
  {
    path: "/basket",
    element: <Basket />,
  },
  {
    path: "/orders",
    element: <OrderHistory />,
  },
  {
    path: "/checkout",
    element: <Checkout />,
  },
];

export default AppRoutes;
