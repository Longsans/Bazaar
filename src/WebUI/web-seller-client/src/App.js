import "./App.css";
import { RouterProvider, createBrowserRouter, Route } from "react-router-dom";
import AppRoutes from "./AppRoutes";

function App() {
  const router = createBrowserRouter(AppRoutes);

  return <RouterProvider router={router} />;
}

export default App;
