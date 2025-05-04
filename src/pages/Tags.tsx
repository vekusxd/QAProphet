import { ConfigProvider, Layout } from "antd"
import { Content, Footer, Header } from "antd/es/layout/layout"
import Sider from "antd/es/layout/Sider"
import { Header as MyHeader } from "../compoinents/header/Header"

export const Tags = () => {
   return (
      <ConfigProvider
         theme={{
            components: {
               Layout: {
                  headerHeight: "fit-content"
               }
            }
         }}
      >
         <Layout style={{ height: '100%', backgroundColor: "#202020" }}>
            <Sider width="15%" style={{ background: '#202020', borderRight: '2px solid red' }}>Sider</Sider>
            <Layout style={{ background: '#202020' }}>
               <Header style={{ background: '#202020' }}>
                  <MyHeader />
               </Header>
               <Content style={{ background: '#202020' }}></Content>
            </Layout>
         </Layout></ConfigProvider>
   )
}
