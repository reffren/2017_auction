var dateAr = dt.split('-');
var newDate = dateAr[1] + '/' + dateAr[0] + '/' + dateAr[2];

var end = new Date(dateAr[1] + '/' + dateAr[0] + '/' + dateAr[2]);

var _second = 1000;
var _minute = _second * 60;
var _hour = _minute * 60;
var _day = _hour * 24;
var timer;

function showRemaining() {
    var now = new Date();
    var distance = end - now;
    if (distance < 0) {
        clearInterval(timer);
        document.getElementById('timecountdown').innerHTML = 'Торги завершены!';
        window.location.href = '/Product/List';

        return;
    }
    var _days = Math.floor(distance / _day);
    var _hours = Math.floor((distance % _day) / _hour);
    var _minutes = Math.floor((distance % _hour) / _minute);
    var _seconds = Math.floor((distance % _minute) / _second);

    document.getElementById('timecountdown').innerHTML = _days + ' д. ';
    document.getElementById('timecountdown').innerHTML += _hours + ' ч. ';
    document.getElementById('timecountdown').innerHTML += _minutes + ' мин.';
}
timer = setInterval(showRemaining, 1000);