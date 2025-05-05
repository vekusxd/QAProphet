import App from "./App.tsx";
import "./index.css";
import {
   createBrowserRouter,
   RouterProvider,
} from "react-router";
import { Tags } from "./pages/Tags/Tags.tsx";
const router = createBrowserRouter([
   {
      path: "/",
      element: <App />,
   }, {
      path: "/tags",
      element: <Tags />,
   },
]);
export default function AppEntrypoint() {
   return (

      <RouterProvider router={router} />
   )
}