// Dữ liệu đầu vào
var FirstName = [
    "GiaBao", "GiaHung", "AnhMinh", "GiaHuy", "GiaKhang", "GiaHao", "GiaBach", "GiaThinh",
    "MyAi", "MyAn", "MyBich", "MyChi", "MyDuyen", "MyHa", "MyHanh",
    "KieuNhu", "KieuThao", "KieuThuy", "KieuTrang", "KieuTrinh", "KieuLan", "KieuLy",
    "NgocBich", "NgocDiep", "NgocHa", "NgocHanh", "NgocHoa", "NgocLan", "NgocMai",
    "NgocMinh", "NgocNgan", "NgocPhuong", "NgocQuynh", "NgocTam", "NgocThao", "NgocThuy",
    "MinhPhuong", "MinhChau", "MinhChinh", "MinhHanh", "MinhHieu", "MinhHoang",
    "TruongVinh", "TruongHieu", "TruongHanh", "TruongHoang", "TruongKhoa", "TruongPhat",
    "TruongQuoc", "TruongSon", "TruongThang", "TruongThinh", "TruongThuan", "TruongToan",
    "TuanAnh", "TuanKiet", "TuanKhoa", "TuanLong", "TuanMinh", "TuanPhat", "TuanHien",
    "DieuQuang", "DieuThao", "DieuThuy", "DieuTrang", "DieuTrinh", "DieuLan",
];

var LastName = [
    "Pham", "Nguyen", "Tran", "Le", "Do", "Ngo", "Kieu", "Vu", "Lam", "Kieu",
    "Quanh", "Bui", "Phan", "Vo"
];

var address = ["Phường 17 - Quận Gò Vấp", "Phường 16 - Quận Gò Vấp", "Phường Hưng Lợi - Quận Ninh Kiều", "Phường An Khánh - Quận Ninh Kiều", "Phường An Bình - Quận Ninh Kiều", "Phường An Hòa - Quận Ninh Kiều", "Phường An Lạc - Quận Ninh Kiều", "Phường An Phú - Quận Ninh Kiều", "Phường An Thới - Quận Ninh Kiều", "Phường An Thạnh - Quận Ninh Kiều", "Phường An Thới - Quận Ninh Kiều", "Phường An Khánh - Quận Ninh Kiều"];

var pwd = "pwd123";
 
var Email = [
    "@gmail.com", "@student.vn.enu", "@yahoo.com"
];

//Mã hóa mật khẩu
const bcrypt = require('bcrypt');
const saltRounds = 10;
const pwdHash = bcrypt.hashSync(pwd, saltRounds);

//Hàm tạo số điện thoại ngẫu nhiên
function generatePhoneNumber(){
    var characters = '0123456789';
    var phoneNumber = '0';
    for(var i = 0; i < 9; i++){
        phoneNumber += characters.charAt(Math.floor(Math.random() * characters.length))
    }

    return phoneNumber;
}

//Hàm tạo tên và email ngẫu nhiên
function generateNameAndEmail(){
    var firstName = FirstName[Math.floor(Math.random() * FirstName.length)];
    var lastName = LastName[Math.floor(Math.random() * LastName.length)];
    var email = firstName + lastName + Email[Math.floor(Math.random() * Email.length)];

    return {
        name: lastName+firstName,
        email: email
    };
}

//Tạo ngày sinh ngẫu nhiên
function generateDateofBirth(){
    var start = new Date(1990,0,1);
    var end = new Date(2008,0,1);

    var date = new Date(start.getTime() + Math.random() * (end.getTime() - start.getTime()))

    return date.toISOString().split('T')[0];
}

//Tạo UID ngẫu nhiên với tối đa là 18 kỹ tự
function UID_Short(){
    var characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    var uid = "";
    for(var i = 0; i < 18; i++){
        uid += characters.charAt(Math.floor(Math.random() * characters.length));
    }
    return uid;
}

//Hàm thêm record vào file .csv, .tsv
const fs = require('fs');
const {parse} = require('csv-parse');
const tsv = require('tsv');

//Hàm chèn số dòng vào file .csv dựa trên số lượng record
function insertRecordToCSV(number){
    var records = [];
    for(var i = 0; i < number; i++){
        
        var Name_email = generateNameAndEmail();
        records.push([
            UID_Short(),
            Name_email.name,
            generateDateofBirth(),
            address[Math.floor(Math.random() * address.length)],
            generatePhoneNumber(),
            Name_email.email,
            pwdHash,
            0,
            0
        ]);
    }
    
    //thêm vào file .csv
    fs.appendFile('UserDataTemp.csv', records.map(row => row.join(',')).join('\n') + '\n', function (err) {
        if (err) throw err;
        console.log('Record added to file!');
    });
}

function insertRecordToTSV(number){
    var records = [];
    for(var i = 0; i < number; i++){
        
        var Name_email = generateNameAndEmail();
        records.push([
            UID_Short(),
            Name_email.name,
            generateDateofBirth(),
            address[Math.floor(Math.random() * address.length)],
            generatePhoneNumber(),
            Name_email.email,
            pwdHash,
            0,
            0
        ]);
    };

    //Thêm vào file .tsv
    TSV  = tsv.stringify(records);
    fs.appendFile('UserData.tsv', TSV, function (err) {
        if (err) throw err;
        console.log('Record added to file!');
    });
}

insertRecordToCSV(15);