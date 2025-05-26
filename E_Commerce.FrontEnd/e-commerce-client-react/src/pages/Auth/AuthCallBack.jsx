import React, {useEffect, useState} from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import { isAuthenticated } from "../../utils/tokens_management";
import LoadingSpinner from "../../components/Ui/LoadingSpinner";


// Xử lý chuyển hướng từ Google OAuth
const AuthCallBack = () =>{
    const [searchParams] = useSearchParams();
    const [error, setError] = useState(null);
    const [isChecking, setIsChecking] = useState(true);
    const navigate = useNavigate();

    useEffect(() =>{
        const CheckAuthStatus = async () =>{
            try{

                //kiểm tra lỗi từ querry parameter
                const errorParam = searchParams.get("error");
                if(errorParam){
                    setError(`Đăng nhập thất bại: ${decodeURIComponent(errorParam)}`);
                    setTimeout(() => navigate("/login"), 3000);
                    return;
                }

                //Đợi một chút để đảm bảo cho cookie được lứu
                await new Promise(resolve => setTimeout(resolve, 500));

                //Kiểm tra xem token được lưu chưa (Cookies)
                if(isAuthenticated()){
                    //Nếu có token thì chuyển hướng về trang chủ
                    navigate("/home");
                }
                else{
                    setError("Đănng nhập thất bại. Vui lòng thử lại sau.");
                    setTimeout(() => navigate("/login"), 3000);
                }
            }
            catch(err){
                console.error("Error in callback:", err);
                setError("Đã xảy ra lỗi. Vui lòng thử lại");
                setTimeout(() => navigate("/login"), 3000);
            }
            finally{
                setIsChecking(false);
            }
        };

        CheckAuthStatus();
    },[navigate, searchParams]);

    if(error){
        return (
            <div className="d-flex flex-column justify-content-center align-items-center vh-100">
                <div className="alert alert-danger">{error}</div>
                <div>Đang chuyển hướng về trang đăng nhập</div>
            </div>
        )
    }
    
    return(
        <div className="d-flex flex-column justify-content-center align-items-center vh-100">
            <LoadingSpinner/>
            <div className="mt-3">Đang xử lý đăng nhập...</div>
            {isChecking && <div className="text-muted small mt-2">Đang kiểm tra thông tin xác thực...</div>} 
        </div>
    )
};

export default AuthCallBack;


