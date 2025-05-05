import App from "./App.tsx";
import "./index.css";
import { StyleProvider } from '@ant-design/cssinjs';
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
      <StyleProvider layer >
         <RouterProvider router={router} />
      </StyleProvider>
   )
}