
$(document).ready(function () {
    $(".register_box_btn").click(function () {
        $(".login_box").hide();
        $(".Register_box").show();
    });

    $(".Login_box_btn").click(function () {
        $(".Register_box").hide();
        $(".login_box").show();
    });

    //$(".Send_OTP_btn").click(function () {
    //    $(".Send_OTP_box").hide();
    //    $(".Confirm_Password_box").hide();
    //    $(".get_OTP_box").show();
    //});

    //$(".OTP_Verification_btn").click(function () {
    //    $(".Send_OTP_box").hide();
    //    $(".get_OTP_box").hide();
    //    $(".Confirm_Password_box").show();
    //});

    //$(".Confirm_Password_btn").click(function () {
    //    $(".Confirm_Password_box").hide();
    //    $(".Send_OTP_box").show();
    //    $(".get_OTP_box").hide();
    //    window.location.href = "/Dealer/login";
    //});
});


function admSelectCheck(nameSelect) {
    if (nameSelect) {
        admOptionValue = document.getElementById("other_refrence").value;
        if (admOptionValue == nameSelect.value) {
            $(".Refrence_Name_box").show();
        }
        else {
            $(".Refrence_Name_box").hide();
        }
    }
    else {
        $(".Refrence_Name_box").hide();
    }
}