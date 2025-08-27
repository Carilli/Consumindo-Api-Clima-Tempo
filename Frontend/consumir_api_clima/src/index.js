import React from 'react';
import ReactDOM from 'react-dom/client';
import './View/index.css';
import App from './View/App';
import reportWebVitals from './View/reportWebVitals';

const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(
    <React.StrictMode>
        <App />
    </React.StrictMode>
);

reportWebVitals();