import { ConfigProvider, Dropdown, Layout } from "antd"
import { Content, Header } from "antd/es/layout/layout"
import { DownOutlined } from '@ant-design/icons';
import Sider from "antd/es/layout/Sider"
import { Header as MyHeader } from "../../components/Header/Header"
import Search from 'antd/es/input/Search'
import './tags.css'
import { useState } from "react"
import { Tag } from "../../components/Tag/Tag";
import Aside from "../../components/Aside/Aside";
export const Tags = () => {
   const [filter, setFilter] = useState(1)

   const tags = [
      {
         title: 'web',
         description: 'Тег можно использовать для вопросов, связанных с разработкой сайтов.',
      }, {
         title: 'web',
         description: 'Тег можно использовать для вопросов, связанных с разработкой сайтов.',
      }, {
         title: 'web',
         description: 'Тег можно использовать для вопросов, связанных с разработкой сайтов.',
      }, {
         title: 'web',
         description: 'Тег можно использовать для вопросов, связанных с разработкой сайтов.',
      }, {
         title: 'web',
         description: 'Тег можно использовать для вопросов, связанных с разработкой сайтов.',
      }, {
         title: 'web',
         description: 'Тег можно использовать для вопросов, связанных с разработкой сайтов.',
      }, {
         title: 'web',
         description: 'Тег можно использовать для вопросов, связанных с разработкой сайтов.',
      }, {
         title: 'web',
         description: 'Тег можно использовать для вопросов, связанных с разработкой сайтов.',
      }, {
         title: 'web',
         description: 'Тег можно использовать для вопросов, связанных с разработкой сайтов.',
      }, {
         title: 'web',
         description: 'Тег можно использовать для вопросов, связанных с разработкой сайтов.',
      },
   ]

   const items = [
      {
         key: '1',
         label: (
            <div
               onClick={() => {
                  setFilter(1)
               }}
               style={{ color: "#e4e4e4" }}
            >
               По популярности
            </div>
         ),
      }, {
         key: '2',
         label: (
            <div
               onClick={() => {
                  setFilter(2)
               }}
               style={{ color: "#e4e4e4" }}
            >
               По названию
            </div>
         ),
      }, {
         key: '3',
         label: (
            <div
               onClick={() => {
                  setFilter(3)
               }}
               style={{ color: "#e4e4e4" }}
            >
               По дате добавления
            </div>
         ),
      },
   ];

   return (
      <ConfigProvider
         theme={{
            components: {
               Layout: {
                  headerHeight: "fit-content"
               }, Input: {
                  colorBorder: "#474747",
                  colorBgContainer: "transparent",
                  hoverBorderColor: "#915eff",
                  colorTextDescription: '#727272',
                  colorPrimaryHover: '#915eff',
                  activeBorderColor: '#702DFF',
                  colorText: '#e4e4e4',
                  colorTextPlaceholder: '#e4e4e48b'
               },
               Button: {
                  colorBorder: "#474747",
                  colorBgContainer: "transparent",
                  defaultHoverBorderColor: '#915eff',
               },
               Dropdown: {
                  colorBgElevated: '#363636',
                  fontFamily: 'Inter',
                  controlItemBgActive: '#702DFF',
                  controlItemBgActiveHover: '#915eff',
                  colorBgTextActive: '#e4e4e4'
               }
            }
         }}
      >
         <Layout style={{ height: '100%', backgroundColor: "#202020" }}>
            <Sider width="15%" style={{ background: '#202020', borderRight: '2px solid red' }}>
               <Aside />
            </Sider>
            <Layout style={{ background: '#202020' }}>
               <Header style={{ background: '#202020' }}>
                  <MyHeader />
               </Header>
               <Content style={{ background: '#202020' }}>
                  <div className="flex flex-col" style={{ padding: '0 50px' }}>
                     <div style={{ color: '#E4E4E4', fontFamily: 'Inter' }} className="mb-[3vw]">
                        <h2 className="text-2xl font-medium mb-[1vw]" >Теги</h2>
                        <p className="text-xs w-1/2">Тег —  это метка или ключевое слово, с помощью которого можно найти контент в интернете. Метки объясняют, что это за контент и о чём он.</p>
                     </div>
                     <div className="flex items-center mb-[3vw]">
                        <Search placeholder="Поиск по тегам"
                           allowClear
                           size="large"
                           style={{ color: '#e4e4e4', width: '25%', marginRight: '2vw' }}
                        />
                        <Dropdown
                           menu={{
                              items,
                              selectable: true,
                              defaultSelectedKeys: ['1']
                           }}
                           placement="bottomLeft"
                        > <div onClick={(e) => e.preventDefault()} className='flex items-center w-fit' style={{ color: '#727272' }}>
                              {items[filter - 1].label.props.children}
                              <span className='h-fit w-fit pt-1 pl-1'>
                                 <DownOutlined />
                              </span>
                           </div></Dropdown>
                     </div>
                     <div className="grid grid-cols-4 gap-10">
                        {tags.map((item, index) => {
                           return <Tag title={item.title} description={item.description} key={index} />
                        })}
                     </div>
                  </div>

               </Content>
            </Layout>
         </Layout></ConfigProvider>
   )
}
