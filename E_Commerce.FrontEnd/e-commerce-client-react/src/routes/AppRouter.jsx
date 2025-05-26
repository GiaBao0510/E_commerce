import React from "react";
import {Route, Routes, Navigate} from 'react-router-dom';
import {isAuthenticated} from "../utils/tokens_management";
import MainLayout from "../components/Layout/MainLayout";
import HomePages from "../pages/Home/HomePages";
import LoginPage from "../pages/Login/LoginPages";
import AuthCallback from "../pages/Auth/AuthCallBack";

//Kiểm tra tài khoan đã đăng nhập hay chưa. Nếu chưa thì chuyển hướng về trang đăng nhập
const ProtectedRoute = ({children}) =>{
    const auth = isAuthenticated();

    if(!auth){
        return <Navigate to="/login" replace />;
    }

    return children;
};


//Định nghĩa các route cho ứng dụng
const AppRoutes = () =>{
    return(
        <Routes>

            <Route path="/login" element={
                <MainLayout>
                    <LoginPage />
                </MainLayout>
            }/>

            <Route path="/auth/callback" element={ 
                <AuthCallback/> 
            }/>

            <Route path="/home" element={
                <ProtectedRoute>
                    <MainLayout>
                        <HomePages />
                    </MainLayout>
                </ProtectedRoute>
            }/>

            {/*Mặc định là sẽ chuyển hướng đến trang chủ. Nếu chưa thì sẽ chuyển hướng đến trang đăng nhập*/}
            <Route path="/" element={
                isAuthenticated() ?
                <Navigate to="/home" replace/>:<Navigate to="/login" replace/>
            }/>

            <Route path="*" element={
                <MainLayout>
                    <div className="text-center my-5">
                        <h1 className="display-4">404</h1>
                        <p className="lead">Page Not Found</p>
                        <a href="/home" className="btn btn-primary">Go to Home</a>
                    </div>
                </MainLayout>
            }/>
        </Routes>
    );
};

export default AppRoutes;