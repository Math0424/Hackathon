import React from "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";

import Cow from './Cow';
import Login from './login';

const App = () => {
    return (
        <div>
            <Router>
                <Routes>
                    <Route path="/" element={<Login />} />
                    <Route path="/Cow" element={<Cow />} />
                </Routes>
            </Router>
        </div>
    );
};

export default App;