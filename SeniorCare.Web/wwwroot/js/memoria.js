// Utilidades
const $ = (sel) => document.querySelector(sel);
const tablero = $("#tablero");
const btnIniciar = $("#btnIniciar");
const btnReiniciar = $("#btnReiniciar");
const selDificultad = $("#dificultad");
const lblMov = $("#movimientos");
const lblTiempo = $("#tiempo");
const lblPares = $("#pares");
const mensajeFin = $("#mensajeFin");

let filas = 4, cols = 4; // por defecto 4x4
let valores = [];
let primera = null, segunda = null;
let bloqueando = false;
let movimientos = 0, pares = 0, totalPares = 0;
let timer = null, segundos = 0;

const EMOJIS = ["🍎", "🍌", "🍇", "🍊", "🍓", "🥝", "🍉", "🍒", "🍍", "🥥", "🥑", "🥕", "🍆", "🌽", "🍪", "🍩", "🧁", "🍰", "🍔", "🍕", "🌮", "🍗", "🍣", "🍤"];

function setGrid() {
    tablero.style.gridTemplateColumns = `repeat(${cols}, var(--card-size))`;
}

function formatearTiempo(s) {
    const m = Math.floor(s / 60);
    const r = s % 60;
    return `${String(m).padStart(2, "0")}:${String(r).padStart(2, "0")}`;
}

function iniciarTimer() {
    detenerTimer();
    segundos = 0;
    lblTiempo.textContent = formatearTiempo(segundos);
    timer = setInterval(() => {
        segundos++;
        lblTiempo.textContent = formatearTiempo(segundos);
    }, 1000);
}

function detenerTimer() {
    if (timer) clearInterval(timer);
    timer = null;
}

function barajar(array) {
    for (let i = array.length - 1; i > 0; i--) {
        const j = Math.floor(Math.random() * (i + 1));
        [array[i], array[j]] = [array[j], array[i]];
    }
}

function generarValores() {
    const cantidad = (filas * cols) / 2;
    const pool = EMOJIS.slice(0, Math.max(cantidad, 12));
    const pares = pool.slice(0, cantidad);
    const cartas = [...pares, ...pares];
    barajar(cartas);
    return cartas;
}

function crearCarta(valor, idx) {
    const btn = document.createElement("button");
    btn.className = "carta";
    btn.type = "button";
    btn.setAttribute("aria-label", "Carta de memoria");
    btn.dataset.valor = valor;
    btn.dataset.index = idx;

    const frente = document.createElement("span");
    frente.className = "frente";
    frente.textContent = valor;

    const dorso = document.createElement("span");
    dorso.className = "dorso";
    dorso.textContent = "🎴";

    btn.appendChild(frente);
    btn.appendChild(dorso);

    btn.addEventListener("click", () => manejarClick(btn));
    btn.addEventListener("keyup", (e) => {
        if (e.key === "Enter" || e.key === " ") manejarClick(btn);
    });

    return btn;
}

function manejarClick(btn) {
    if (bloqueando || btn.classList.contains("bloqueada") || btn === primera) return;

    revelar(btn);

    if (!primera) { primera = btn; return; }

    segunda = btn;
    movimientos++;
    lblMov.textContent = movimientos;

    evaluar();
}

function revelar(btn) { btn.classList.add("revelada"); }
function ocultar(...btns) { btns.forEach(b => b.classList.remove("revelada", "fallo")); }
function bloquear(...btns) { btns.forEach(b => b.classList.add("bloqueada", "acierto")); }

function evaluar() {
    const v1 = primera.dataset.valor;
    const v2 = segunda.dataset.valor;

    if (v1 === v2) {
        bloquear(primera, segunda);
        pares++;
        lblPares.textContent = `${pares}/${totalPares}`;
        primera = segunda = null;

        if (pares === totalPares) {
            detenerTimer();
            mensajeFin.classList.remove("d-none");
            mensajeFin.textContent = `¡Muy bien! Terminaste en ${movimientos} movimientos y ${formatearTiempo(segundos)}.`;
            btnReiniciar.disabled = false;
        }
    } else {
        bloqueando = true;
        primera.classList.add("fallo");
        segunda.classList.add("fallo");
        setTimeout(() => {
            ocultar(primera, segunda);
            primera = segunda = null;
            bloqueando = false;
        }, 700);
    }
}

function prepararTablero() {
    tablero.innerHTML = "";
    setGrid();
    valores = generarValores();
    totalPares = valores.length / 2;
    lblPares.textContent = `0/${totalPares}`;
    movimientos = 0;
    lblMov.textContent = "0";
    mensajeFin.classList.add("d-none");
    btnReiniciar.disabled = true;

    valores.forEach((v, i) => tablero.appendChild(crearCarta(v, i)));
}

function aplicarDificultad() {
    const dif = parseInt(selDificultad.value, 10);
    cols = dif; // 4, 5 o 6 columnas; filas se mantiene 4 para legibilidad
    filas = 4;
}

// >>> Nuevas funciones para evitar .click()
function empezarJuego() {
    aplicarDificultad();
    prepararTablero();
    iniciarTimer();
}
function reiniciarJuego() {
    aplicarDificultad();
    prepararTablero();
    iniciarTimer();
}

btnIniciar.addEventListener("click", empezarJuego);
btnReiniciar.addEventListener("click", reiniciarJuego);

// Accesibilidad: teclas rápidas sin usar .click()
document.addEventListener("keydown", (e) => {
    if (e.key === "r" || e.key === "R") reiniciarJuego();
    if (e.key === "n" || e.key === "N") empezarJuego();
});
