import { Catalog } from "./components/Catalog";
import { Home } from "./components/Home";

const AppRoutes = [
  {
    index: true,
    element: <Home />
  },
  {
    path: '/catalog',
    element: <Catalog />
  }
];

export default AppRoutes;
