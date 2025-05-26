import axios from "axios";

// Tạo instance axios riêng cho token operations
const tokenAPI = axios.create({
    baseURL: process.env.REACT_APP_BASE_URL,
    timeout: 30000,
    headers: {
        'Content-Type': 'application/json',
        'Accept': 'application/json'
    },
    withCredentials: true
});

/**  Hàm lấy token từ cookie dựa trên tên token cần lấy
    * @param {string} name - Tên token cần lấy
    * @returns {string|null} - Giá trị token nếu tìm thấy, ngược lại trả về null
*/
export const getToken = (name) =>{
    try {
        const value = `; ${document.cookie}`;
        const parts = value.split(`; ${name}=`);
        if(parts.length === 2) {
            return parts.pop().split(';').shift();
        }
        return null;
    } catch (error) {
        console.error("Error getting token from cookie:", error);
        return null;
    }
}

/**hàm đặt cookie với các tùy chọn
 * @param {string} name - Tên cookie cần lưu
 * @param {string} value - Giá trị cookie cần lưu
 * @param {object} option - Các tùy chọn cho cookie
 * */ 
export const setCookie = (name, value, option = {}) =>{
    try {
        const {
            path = '/',
            expires = null,
            maxAge = null,
            domain = null,
            secure = true,
            sameSite = 'Strict'
        } = option;

        let cookieString = `${name}=${value}; path=${path}`;
        
        //Thiết lập trạng thái nếu có
        if(expires) cookieString += `; expires=${expires.toUTCString()}`;
        if(maxAge !== null) cookieString += `; max-age=${maxAge}`;
        if(domain) cookieString += `; domain=${domain}`;
        if(secure) cookieString += `; secure`;
        if(sameSite) cookieString += `; samesite=${sameSite}`;

        document.cookie = cookieString;
        return true;
    } catch (error) {
        console.error(`Error setting cookie: ${name}`, error);
        return false;
    }
}

/**
 * Hàm phân tích chuỗi cookie từ server để trích xuất thông tin
 * @param {string} cookieString - Chuỗi cookie từ server 
 * @returns {object} - đối tượng chứa thông tin cookie
 */
export const parseCookieString = (cookieString) =>{
    try {

        if(!cookieString || typeof cookieString !== 'string')
            throw new Error("Invalid cookie string");

        //Tách chuỗi cookie thành các phần
        //Ví dụ: access_token=abc123; expires=Wed, 21 Oct 2023 07:28:00 GMT; path=/; domain=example.com; secure; samesite=strict
        const parts = cookieString.split(';');
        const nameValue = parts[0].split('=');

        //Tấy thông tin tên cookie và giá trị ở đằng trước
        const name = nameValue[0].trim();
        const value = nameValue[1].trim();

        const options = {};
        parts.slice(1).forEach( part => {
            
            //Tách tên và giá trị của từng phần
            const [key, val] = part.split('=').map(item => item.trim());
            if(key.toLocaleLowerCase() === 'expires'){
                options.expires = new Date(val);
            } else if(key.toLocaleLowerCase() === 'max-age'){
                options.maxAge = parseInt(val);
            } else if(key.toLocaleLowerCase() === 'path'){
                options.path = val;
            } else if(key.toLocaleLowerCase() === 'domain'){
                options.domain = val;
            } else if(key.toLocaleLowerCase() === 'secure'){
                options.domain = true;
            } else if(key.toLocaleLowerCase() === 'samesite'){
                options.domain = val;
            }
        });

        return {name, value, options};
    } catch (error) {
        console.error("Error parsing cookie string:", error);
        return null;        
    }
}

// Lưu accessToken vào cookie
export const saveAccessToken = (token, options = {}) =>{

    if(!token){
        console.error("Token is null or undefined");
        return false;
    }
    
    //Nếu token là chuối cookie từ server
    if(token.includes('=') && token.includes(';')){
        const {name, value, options: cookieOptions} = parseCookieString(token);
        if(name === 'access_token'){
            setCookie(name, value, {...options, ...cookieOptions});
        }
        return false;
    }

    return setCookie('access_token', token, options);
}

export const saveRefreshToken = (token, options = {}) =>{
    
    if(!token){
        console.error("Token is null or undefined");
        return false;
    }

    //Nếu token là chuối cookie từ server
    if(token.includes('=') && token.includes(';')){
        const {name, value, options: cookieOptions} = parseCookieString(token);
        if(name === 'refresh_token'){
            setCookie(name, value, {...options, ...cookieOptions});
        }
        return false;
    }

    //Thiết lập thời gian sống dàu hơn
    const refreshTokenOpt = {
        maxAge: 60 * 60 * 24 * 30, // 30 ngày
        ...options
    };

    return setCookie('refresh_token', token, refreshTokenOpt);
}

// Delete AccessToken & RefreshToken to cookie
export const removeAccessToken = () =>{
    document.cookie = 'access_token=; path=/; expires=Thu, 01 Jan 1970 00:00:00 GMT; secure; samesite=strict';
  return true;
}

export const removeRefreshToken = () =>{
    document.cookie = 'refresh_token=; path=/; expires=Thu, 01 Jan 1970 00:00:00 GMT; secure; samesite=strict';
  return true;
}

// Kiểm tra người dùng đã đăng nhập hay chưa
export const isAuthenticated = () =>{
    return !!getToken('access_token');
}

//Kiểm tra xem accessToken có còn hiệu lực hay không
export const isAccessTokenExpired = () =>{
    const token = getToken('access_token');

    if(!token) return true;
    try {
        //Giải mã payload từ JWT (không cần xác thực chữ ký)
        const payload = JSON.parse(atob(token.split('.')[1]));
        return payload.exp * 1000 < Date.now();
    } catch (error) {
        console.error("Error checking access token expiration:", error);
        return true;
    }
}

/**Hàm làm mới token
 * Đầu vào khi gửi trong body là {access_token: access_token ,refresh_token: refreshToken}
*/
export const refreshToken = async () =>{
    try {
        
        const refreshToken = getToken('refresh_token');
        const accessToken = getToken('access_token');
        
        if(!refreshToken) {
            throw new Error("Refresh token not found");
        }

        //Gọi api đển refreh token
        const res = await tokenAPI.post(process.env.REACT_APP_REFRESH_TOKEN, {
            accessToken: accessToken,
            refresh_token: refreshToken
        });
        
        //Kiểm tra cookies - mới đã được lưu tự động bởi trình duyệt chưa
        const newAccessToken = getToken('access_token');
        if(newAccessToken) {
            return newAccessToken;
        }
        else if(res.data && res.data.access_token) {
            //Lưu access token vào cookie
            saveAccessToken(res.data.access_token);
            return res.data.access_token;
        }
        
        throw new Error("No access token found in response");
    } catch (error) {
        console.error("Error refreshing token:", error);
        removeAccessToken();
        removeRefreshToken();
        throw new Error("Error refreshing token:", error);
    }
}