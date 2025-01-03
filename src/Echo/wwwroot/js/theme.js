const changeTheme = (change) => {
    let theme = localStorage.getItem("theme");
    if (change) {
        if (theme == null || theme === "light") {
            theme = "dark";
        }
        else {
            theme = "light";
        }
    }

    const button = document.getElementById("theme");
    if (button) {
        button.innerText = theme === "dark" ? "☀" : "☽︎";
    }

    localStorage.setItem("theme", theme);

    document.getElementsByTagName("html")[0].className = theme;
    document.body.classList.remove("dark");
    document.body.classList.remove("light");
    document.body.classList.add(theme);
}

changeTheme(false);
