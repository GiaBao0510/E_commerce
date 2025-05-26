import axios from 'axios';
import { 
    getToken, 
    refreshToken,
    removeAccessToken, 
    removeRefreshToken,
    isAccessTokenExpired,
    saveAccessToken,
    saveRefreshToken
} from '../utils/tokens_management';


//Tạo một instace axios với url cơ sở
const API = axios.create({
    baseURL: process.env.REACT_APP_BASE_URL,
    timeout: 10000, //Thời gian chờ 10 giây. Tránh request treo
    headers: {
        'Content-Type': 'application/json',
        'Accept': 'application/json'
    },
    withCredentials: true  // Quan trọng để gửi cookies qua các domain
});

//Thêm interceptor để tự động thêm token vào header
API.interceptors.request.use(
    async (config) => {
        let token = getToken('access_token');

        //Kiểm tra xem token có tồn tại 
        if(token ){

            //Nếu token chưa hết hạn, thêm vào header
            if(!isAccessTokenExpired(token)){
                config.headers.Authorization = `Bearer ${token}`;
            }
            //Nếu token đã hết hạn thì làm mới
            else{
                try {
                    token = await refreshToken();
                    config.headers.Authorization = `Bearer ${token}`;
                } 
                catch (error) {
                    console.error("Error refreshing token:", error);
                    //Nếu không thể làm mới token, xóa token và chuyển hướng đến trang đăng nhập
                    removeAccessToken();
                    removeRefreshToken();
                    window.location.href = '/login';
                }
            }
        }

        return config;
    },
    (error) => {
        return Promise.reject(error);
    }
);

//Thêm interceptors để xử lý khi token hết hạn
API.interceptors.response.use(
    (res) => res,
    async (error) =>{

        const originalRequest = error.config;

        //Nếu lỗi  401 (Unauthorized)
        if(error.response.status === 401 && !originalRequest._retry){
            originalRequest._retry = true;

            try 
            {
                //Thử để làm mới token
                await refreshToken();

                //Sau khi làm mới thành công, gửi lại request ban đầu
                originalRequest.headers.Authorization = `Bearer ${getToken('access_token')}`;
            } catch (refreshToken_error) {
                //Nếu làm mới token thất bại thì, chuyển về trang login
                removeAccessToken();
                removeRefreshToken();
                return Promise.reject(refreshToken_error);
            }
        }

        return Promise.reject(error);
    }
);

//Đăng nhập
export const Login = async (phone_num, pass_word) =>{
    try {
        const res = await API.post(process.env.REACT_APP_LOGIN, {
            phone_num: phone_num,
            pass_word: pass_word
        });

        //Hiển thị token
        console.log("Access Token:", getToken('access_token'));
        console.log("Refresh Token:", getToken('refresh_token'));

        return res.data;
    } catch (error) {

        console.error("Error during login:", error);

        //Lấy thông tin lỗi cụ thể
        if(error.response){
            console.error(`Server response error: \n 
                Status: ${error.response.status}, \n
                Data: ${error.response.data}, \n
                Headers: ${error.response.headers}, \n
                Config: ${error.config}, \n
                `
            );
            throw new Error(error.response.data?.message || "Đăng nhập thất bại");
        }else if(error.request){
            // Nếu không nhận được phản hồi từ server
            console.error("No response received:", error.request);
            throw new Error("Không thể kết nối đến server");
        }
        else {
            // Lỗi trong quá trình thiết lập request
            console.error("Request setup error:", error.message);
            throw new Error("Lỗi kết nối: " + error.message);
        }      
    }
}

//Đăng xuất
export const Logout = async() =>{
    try{
        //Gọi api để đăng xuất  (nếu có)
        removeAccessToken();
        removeRefreshToken();
    }catch(error){
        console.error("Error during logout:", error);
        throw error;
    }
}

export default API;
