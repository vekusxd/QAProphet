import { createRoot } from "react-dom/client";
import { StrictMode, lazy, Suspense } from "react";
import { KcPage, type KcContext } from "./keycloak-theme/kc.gen";
const AppEntrypoint = lazy(() => import("./main.app"));

// The following block can be uncommented to test a specific page with `yarn dev`
// Don't forget to comment back or your bundle size will increase
/*
import { getKcContextMock } from "./keycloak-theme/login/KcPageStory";

if (import.meta.env.DEV) {
    window.kcContext = getKcContextMock({
        pageId: "register.ftl",
        overrides: {}
    });
}
*/

createRoot(document.getElementById("root")!).render(
   <StrictMode>
      {window.kcContext ? (
         <KcPage kcContext={window.kcContext} />
      ) : (
         <Suspense>
            <AppEntrypoint />
         </Suspense>
      )}
   </StrictMode>
);

declare global {
   interface Window {
      kcContext?: KcContext;
   }
}