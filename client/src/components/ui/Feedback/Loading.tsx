import React from "react";

const Loading = () => {
  return (
    <div className="flex justify-center items-center">
      <div className="p-8">
        <div className="animate-spin rounded-full size-8 border-b-2 border-blue-600 mx-auto"></div>
        <p className="mt-2 text-gray-500">Loading...</p>
      </div>
    </div>
  );
};

export default Loading;
