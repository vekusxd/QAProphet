import { ConfigProvider } from 'antd';
import React from 'react'
import { NavLink, useLocation } from 'react-router';
export default function Aside() {
   const location = useLocation()
   const tags = [
      {
         pathname: '/',
         title: 'Главная',
         icon: '/src/components/Aside/assets/homeIcon.png'
      }, {
         pathname: '/tags',
         title: 'Теги',
         icon: '/src/components/Aside/assets/tagsIcon.png'
      }, {
         pathname: '/',
         title: 'Вопросы',
         icon: '/src/components/Aside/assets/questionsIcon.png'
      }, {
         pathname: '/',
         title: 'Избранное',
         icon: '/src/components/Aside/assets/favouriteIcon.png'
      }, {
         pathname: '/',
         title: 'Сообщество',
         icon: '/src/components/Aside/assets/communityIcon.png'
      }, {
         pathname: '/',
         title: 'Уведомления',
         icon: '/src/components/Aside/assets/notificationIcon.png'
      }, {
         pathname: '/',
         title: 'Настройки',
         icon: '/src/components/Aside/assets/settingsIcon.png'
      },
   ]
   return (
      <ConfigProvider theme={{ hashed: false }}>

         <div className='p-4 h-full flex flex-col'>
            <div className='flex items-center w-full justify-between mb-[4vw]'>
               <p className='font-base font-medium text-[#e4e4e4]' style={{ fontFamily: 'Inter' }}>QAProphet</p>
               <img src='/src/assets/logo.png' />
            </div>
            <div className='flex flex-col items-between justify-between h-full'>
               <div className='flex flex-col items-between gap-y-2'>
                  {tags.map((item, index) => {
                     return <div className={`flex items-center p-2 cursor-pointer rounded-md ${location.pathname == item.pathname ? 'bg-[#702DFF] hover:bg-[#915eff] text-[#e4e4e4]' : 'hover:brightness-160'} transition-all duration-0.25`} key={index}>
                        <img src={item.icon} alt="" className={`w-6 h-6 mr-[0.5vw]  ${location.pathname == item.pathname ? 'brightness-0 invert' : ''}`} />
                        <NavLink to={{ pathname: item.pathname }}
                           className={`text-[#727272] text-base font-medium  ${location.pathname == item.pathname ? 'text-[#e4e4e4]' : ''}`}
                           style={{ fontFamily: 'Inter' }}
                           data-active={false}
                        >{item.title}</NavLink>
                     </div>
                  })}

               </div>
               <div className='flex flex-col items-between'>
                  <div className={`flex items-center p-2 cursor-pointer rounded-md hover:brightness-160 transition-all duration-0.25`} >
                     <img src='/src/components/Aside/assets/settingsIcon.png' alt="" className={`w-6 h-6 mr-[0.5vw]  `} />
                     <NavLink to={{ pathname: '/' }}
                        className={`text-[#727272] text-base font-medium  `}
                        style={{ fontFamily: 'Inter' }}
                        data-active={false}
                     >Настройки</NavLink>
                  </div>
                  <div className={`flex items-center p-2 cursor-pointer rounded-md hover:brightness-160 transition-all duration-0.25`} >
                     <img src='/src/components/Aside/assets/logoutIcon.png' alt="" className={`w-6 h-6 mr-[0.5vw]`} />
                     <NavLink to={{ pathname: '/' }}
                        className={`text-[#727272] text-base font-medium  `}
                        style={{ fontFamily: 'Inter' }}
                        data-active={false}
                     >Выйти</NavLink>
                  </div>
               </div>
            </div>

         </div>
      </ConfigProvider>
   )
}
