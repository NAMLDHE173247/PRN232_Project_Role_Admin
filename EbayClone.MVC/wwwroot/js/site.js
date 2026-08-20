(() => {
  const root = document.documentElement;
  const saved = localStorage.getItem("admin-theme");
  const preferredDark = window.matchMedia?.("(prefers-color-scheme: dark)").matches;
  root.dataset.theme = saved ?? (preferredDark ? "dark" : "light");
  document.getElementById("theme-toggle")?.addEventListener("click", () => {
    root.dataset.theme = root.dataset.theme === "dark" ? "light" : "dark";
    localStorage.setItem("admin-theme", root.dataset.theme);
  });
})();
