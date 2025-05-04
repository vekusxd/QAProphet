import { Avatar, Dropdown } from 'antd'
import { DownOutlined } from '@ant-design/icons';
import Search from 'antd/es/input/Search'
import React from 'react'
import MyAvatar from '../../assets/avatar.png'
import { ConfigProvider } from "antd";
import { Link } from 'react-router';



export function Header() {

   const items: MenuProps['items'] = [
      {
         key: '1',
         label: (
            <Link to={{ pathname: '/' }}>
               В меню
            </Link>
         ),
      }, {
         key: '2',
         label: (
            <Link to={{ pathname: '/' }}>
               На хуй?
            </Link>
         ),
      },
   ];


   return (
      <ConfigProvider
         theme={{
            components: {
               Input: {
                  colorBorder: "#474747",
                  colorBgContainer: "transparent",
                  hoverBorderColor: "#915eff",
                  colorTextDescription: '#474747',
                  colorPrimaryHover: '#915eff',
                  activeBorderColor: '#702DFF',
                  colorText: '#e4e4e4'
               },
               Button: {
                  colorBorder: "#474747",
                  colorBgContainer: "transparent",
                  defaultHoverBorderColor: '#915eff',
               },
               Dropdown: {
                  colorText: "#e4e4e4",
                  colorTextDescription: '#e4e4e4',
                  colorBgElevated: '#363636',
                  fontFamily: 'Inter'
               }
            }
         }}
      >
         <div className='flex w-full py-8 gap-x-10 items-center'>
            <Search
               placeholder="input search text"
               allowClear
               size="large"
               style={{ color: '#e4e4e4' }}
            />
            <div className='flex items-center min-w-[20%] w-fit gap-x-2'>
               <div className='min-w-fit min-h-fit'>
                  <Avatar
                     size={40}
                     src={MyAvatar}
                  />
               </div>
               <Dropdown
                  menu={{
                     items,
                  }}
                  placement="bottomLeft"
                  dropdownRender={(menu) => (
                     <div className='mt-2'>
                        {React.cloneElement(menu as React.ReactElement<{}>)}
                     </div>
                  )}
               >
                  <a onClick={(e) => e.preventDefault()} className='flex items-center w-full ' style={{ color: '#e4e4e4' }}>
                     supertatar
                     <span className='h-fit w-fit pt-1 pl-1'>
                        <DownOutlined />
                     </span>
                  </a>
               </Dropdown>
            </div>
         </div>
      </ConfigProvider>
   )
}
