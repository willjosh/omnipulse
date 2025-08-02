import React from "react";

interface DetailPanelProps {
  open: boolean;
  onClose: () => void;
  title?: string;
  children: React.ReactNode;
  width?: string;
  className?: string;
}

const DetailPanel: React.FC<DetailPanelProps> = ({
  open,
  onClose,
  title,
  children,
  width = "w-[400px]",
  className = "",
}) => {
  return (
    <div
      className={`fixed inset-0 z-[100] flex ${open ? "" : "pointer-events-none"}`}
      aria-hidden={!open}
    >
      {/* Overlay */}
      <div
        className={`fixed inset-0 bg-black bg-opacity-30 transition-opacity duration-200 ${open ? "opacity-100" : "opacity-0"}`}
        onClick={onClose}
      />
      {/* Panel */}
      <aside
        className={`relative bg-white shadow-xl h-full transition-transform duration-300 ${width} ${open ? "translate-x-0" : "translate-x-full"} ${className}`}
        style={{ right: 0 }}
      >
        <div className="flex items-center justify-between p-4 border-b">
          <h2 className="text-lg font-semibold">{title}</h2>
          <button
            onClick={onClose}
            aria-label="Close"
            className="text-gray-500 hover:text-gray-700"
          >
            &times;
          </button>
        </div>
        <div className="p-4 overflow-y-auto h-[calc(100%-56px)]">
          {children}
        </div>
      </aside>
    </div>
  );
};

export default DetailPanel;
