import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import "./styles.css";

function App() {
  return (
    <main className="shell">
      <section className="card">
        <p className="eyebrow">GameNet Manager 3</p>
        <h1>Foundation Ready</h1>
        <p>
          UI contracts will consume Server APIs; business decisions stay on
          the Server.
        </p>
        <div className="status">FOUNDATION / NOT CONNECTED TO SERVER</div>
      </section>
    </main>
  );
}

createRoot(document.getElementById("root")!).render(
  <StrictMode><App /></StrictMode>,
);
