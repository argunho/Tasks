
// Send data
function sendData(ev, url, method) {
    var data = $(ev).serialize();
    $.ajax({
        type: method,
        url: '/Actions/' + url,
        data: data,
        success: function (res) {
            if (res) 
                history.back();
        }
    });
    return false;
}

// Handle data
function handleData(url, method) {
    $.ajax({
        type: method,
        url: '/Actions/' + url,
        success: function (res) {
            if (res)
                location.reload();
        }
    });
    return false;
}

// Get search
function getSearchResult() {
    var key = $("#search").val();
    location.href = "/SearchResult?key=" + key;
}