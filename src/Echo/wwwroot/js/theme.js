function changeTheme() {
    let theme = localStorage.getItem("theme");
    if (theme == null || theme === "light") {
        theme = "dark";
    }
    else {
        theme = "light";
    }

    localStorage.setItem("theme", theme);

    document.getElementsByTagName("html")[0].className = theme;
    document.body.classList.remove("dark");
    document.body.classList.remove("light");
    document.body.classList.add(theme);
}

const startTheme = localStorage.getItem("theme");
if (startTheme != null) {
    console.log(startTheme);
    document.getElementsByTagName("html")[0].className = startTheme;
    document.body.classList.add(startTheme);
}
