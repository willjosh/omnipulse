import React, { useEffect } from "react";

export type NotificationType = "success" | "error" | "info" | "warning";

interface NotificationProps {
  type?: NotificationType;
  message: string;
  onClose?: () => void;
  duration?: number; // ms
  className?: string;
}

const typeStyles: Record<NotificationType, string> = {
  success: "bg-green-100 text-green-800 border-green-300",
  error: "bg-red-100 text-red-800 border-red-300",
  info: "bg-blue-100 text-blue-800 border-blue-300",
  warning: "bg-yellow-100 text-yellow-800 border-yellow-300",
};

const Notification: React.FC<NotificationProps> = ({
  type = "info",
  message,
  onClose,
  duration = 3000,
  className = "",
}) => {
  useEffect(() => {
    if (!onClose) return;
    const timer = setTimeout(onClose, duration);
    return () => clearTimeout(timer);
  }, [onClose, duration]);

  return (
    <div
      className={`fixed top-16 right-6 z-[110] px-4 py-3 flex rounded shadow border max-w-md break-words ${typeStyles[type]} ${className}`}
      role="alert"
    >
      <span className="block">{message}</span>
      {onClose && (
        <button
          className="ml-4 text-lg font-bold text-gray-400 hover:text-gray-700 focus:outline-none"
          onClick={onClose}
          aria-label="Close notification"
        >
          ×
        </button>
      )}
    </div>
  );
};

export default Notification;
