namespace E_commerce.Infrastructure.Templates.Email
{
    public static class VerificationForm
    {
        static VerificationForm(){ }
        public static string OTPcodeVerificationForm(string otp){
            return $@"
                <!DOCTYPE html>
                <html lang='vi'>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>VoteSecure - Mã xác thực OTP</title>
                    <style>
                        @import url('https://fonts.googleapis.com/css2?family=Poppins:wght@400;600;700&display=swap');
                        
                        :root {{
                            --primary-color: #4a90e2;
                            --secondary-color: #50c878;
                            --accent-color: #ff6b6b;
                            --text-color: #333;
                            --background-color: #f0f4f8;
                        }}
                        body {{
                            font-family: 'Poppins', sans-serif;
                            background: linear-gradient(120deg, #84fab0 0%, #8fd3f4 100%);
                            display: flex;
                            justify-content: center;
                            align-items: center;
                            min-height: 100vh;
                            margin: 0;
                            padding: 20px;
                            box-sizing: border-box;
                        }}
                        .container {{
                            background: rgba(255, 255, 255, 0.9);
                            padding: 2.5rem;
                            border-radius: 20px;
                            box-shadow: 0 10px 30px rgba(0, 0, 0, 0.1);
                            text-align: center;
                            max-width: 450px;
                            width: 100%;
                            backdrop-filter: blur(10px);
                            animation: fadeIn 0.5s ease-out;
                        }}
                        @keyframes fadeIn {{
                            from {{ opacity: 0; transform: translateY(-20px); }}
                            to {{ opacity: 1; transform: translateY(0); }}
                        }}
                        h1 {{
                            color: #3557FF;
                            margin-bottom: 1rem;
                            font-size: 2.5rem;
                            text-transform: uppercase;
                            letter-spacing: 2px;
                            text-shadow: 2px 2px 4px rgba(0,0,0,0.1);
                            background: linear-gradient(45deg, var(--primary-color), var(--secondary-color));
                            -webkit-background-clip: text;
                            -webkit-text-fill-color: transparent;
                        }}
                        p {{
                            color: var(--text-color);
                            margin-bottom: 1.5rem;
                            line-height: 1.6;
                            font-size: 1.1rem;
                        }}
                        .otp-display {{
                            font-size: 2.2rem;
                            font-weight: bold;
                            color: #EF8924;
                            padding: 15px;
                            background: linear-gradient(45deg, var(--accent-color), var(--primary-color));
                            border-radius: 15px;
                            margin: 25px 0;
                            letter-spacing: 8px;
                            box-shadow: 0 5px 15px rgba(0, 0, 0, 0.2);
                            transition: all 0.3s ease;
                            animation: pulse 2s infinite;
                        }}
                        @keyframes pulse {{
                            0% {{ transform: scale(1); }}
                            50% {{ transform: scale(1.05); }}
                            100% {{ transform: scale(1); }}
                        }}
                        .otp-display:hover {{
                            transform: translateY(-5px);
                            box-shadow: 0 8px 20px rgba(0, 0, 0, 0.3);
                        }}
                        .image-container {{
                            width: 100%;
                            max-width: 300px;
                            margin: 25px auto 0;
                            overflow: hidden;
                            border-radius: 15px;
                            box-shadow: 0 8px 25px rgba(0, 0, 0, 0.2);
                            transition: all 0.3s ease;
                        }}
                        .image-container:hover {{
                            transform: translateY(-10px);
                            box-shadow: 0 12px 30px rgba(0, 0, 0, 0.3);
                        }}
                        .responsive-image {{
                            width: 100%;
                            height: auto;
                            display: block;
                            object-fit: cover;
                            object-position: center;
                            transition: transform 0.3s ease;
                        }}
                        .responsive-image:hover {{
                            transform: scale(1.1);
                        }}
                        @media (max-width: 600px) {{
                            .container {{
                                padding: 2rem;
                            }}
                            h1 {{
                                font-size: 2rem;
                            }}
                            .otp-display {{
                                font-size: 1.8rem;
                                letter-spacing: 6px;
                            }}
                            p {{
                                font-size: 1rem;
                            }}
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <h1>E-Commerce</h1>
                        <p> Dưới đây là mã xác thực (OTP) của bạn:</p>
                        <div class='otp-display'>{otp}</div>
                        <p>Lưu ý rằng mã OTP này chỉ có hiệu lực trong vòng 5 phút kể từ khi email được gửi. Sau thời gian này, mã sẽ hết hạn và bạn cần yêu cầu một mã mới nếu vẫn chưa hoàn thành đăng nhập.</p>
                        <p>Đây là email tự động được gửi từ hệ thống, vui lòng không trả lời trực tiếp vào email này.</p>
                        <div class='image-container'>
                            <img class='responsive-image' src='https://res.cloudinary.com/dkajnklq6/image/upload/v1726309469/NguoiDung/x101hxwcmigiywmodxlo.png' alt='Hình ảnh minh họa' >
                        </div>
                    </div>
                </body>
                </html>";
        }
    }
}