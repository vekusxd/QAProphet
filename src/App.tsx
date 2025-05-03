import { Button } from "antd"
import { Link } from "react-router"

function App() {
   return (
      <div className="absolute top-1/2 left-1/2 bg-red-200 translate-y-[-50%] translate-x-[-50%] flex flex-col gap-10 text-center p-6">
         Sosal?
         <Button>
            <Link to={{ pathname: '/sosalishe' }}>
               Да, было
            </Link>
         </Button>
      </div>
   )
}

export default App
