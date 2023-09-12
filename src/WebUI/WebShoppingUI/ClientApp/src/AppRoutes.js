import { Home } from "./components/Home";
import { Login } from "./components/Login";
import { Logout } from "./components/Logout";
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
];

export default AppRoutes;
