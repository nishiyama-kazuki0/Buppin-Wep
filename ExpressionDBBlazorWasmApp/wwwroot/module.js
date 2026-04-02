export class Rectangle {
    constructor(height, width) {
        this.height = height;
        this.width = width;
    }
}

export function sum(...theArgs) {
    let total = 0;
    for (const arg of theArgs) {
        total += arg;
    }
    return total;
}

export let data = 100;

export function outputLog(obj) {
    console.log(typeof obj, obj);
}

export function getDeviceType() {
    var userAgent = navigator.userAgent || navigator.vendor || window.opera;
    if (/windows phone/i.test(userAgent)) {
        return "Windows Phone";
    }
    if (/android/i.test(userAgent)) {
        return "Android";
    }
    if (/iPad|iPhone|iPod/.test(userAgent) && !window.MSStream) {
        return "iOS";
    }
    return userAgent;// "unknown";
}
