import App from "./App.tsx";
import "./index.css";
import {
   createBrowserRouter,
   RouterProvider,
} from "react-router";
import { Sosalishe } from "./pages/Sosalishe.tsx";
const router = createBrowserRouter([
   {
      path: "/",
      element: <App />,
   }, {
      path: "/sosalishe",
      element: <Sosalishe />,
   },
]);
export default function AppEntrypoint() {
   return (
      <RouterProvider router={router} />
   )
}