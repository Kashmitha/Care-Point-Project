import { BrowserRouter, Routes } from 'react-router-dom';
import Navbar from './components/layout/Navbar';
import './App.css'

function App() {

  return (
    <BrowserRouter>
      <Navbar />
      <Routes>
        {/* your routes here */}
      </Routes>
    </BrowserRouter>
  );
}

export default App
