import { Button } from 'antd'
import { Link } from 'react-router'

export const Sosalishe = () => {
   return (
      <div className="absolute top-1/2 left-1/2 bg-red-200 translate-y-[-50%] translate-x-[-50%] flex flex-col gap-10 text-center p-6">
         Молодец
         <Button>
            <Link to={{ pathname: '/' }}>
               Благодарствую
            </Link>
         </Button>
      </div>
   )
}
