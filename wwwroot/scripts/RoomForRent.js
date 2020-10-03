function postDataToUrlAndHideElementOnSuccess(elementId, dataToPass, urlToPost) {
    $.ajax({
        type: 'POST',
        url: urlToPost,
        data: dataToPass,
        dataType: 'text',
        success: function (msg) {
            let result = JSON.parse(msg);
            if (result.isSuccess == true)
                $('#' + elementId).hide();
            console.log(msg);
        },
        error: function (req, status, error) {
            console.log(msg);
        }
    });
}