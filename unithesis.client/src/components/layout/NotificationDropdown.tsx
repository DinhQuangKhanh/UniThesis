import { useState, useRef, useEffect } from "react";
import { motion, AnimatePresence } from "framer-motion";

// Notification types for different roles
export type UserRole = "admin" | "mentor" | "student" | "evaluator";

interface Notification {
  id: string;
  title: string;
  message: string;
  time: string;
  read: boolean;
  type: "info" | "warning" | "success" | "error";
  icon: string;
}

// Mock notifications for each role
const notificationsByRole: Record<UserRole, Notification[]> = {
  admin: [
    {
      id: "1",
      title: "Yêu cầu hỗ trợ mới",
      message: "Sinh viên Nguyễn Văn A gửi yêu cầu hỗ trợ về đăng ký đề tài",
      time: "5 phút trước",
      read: false,
      type: "warning",
      icon: "support_agent",
    },
    {
      id: "2",
      title: "Đề tài mới cần phê duyệt",
      message: "3 đề tài mới đang chờ phê duyệt từ GVHD",
      time: "1 giờ trước",
      read: false,
      type: "info",
      icon: "pending_actions",
    },
    {
      id: "3",
      title: "Backup hệ thống hoàn tất",
      message: "Sao lưu dữ liệu hệ thống đã hoàn thành thành công",
      time: "2 giờ trước",
      read: true,
      type: "success",
      icon: "backup",
    },
    {
      id: "4",
      title: "Cảnh báo dung lượng",
      message: "Dung lượng lưu trữ đã sử dụng 85%",
      time: "1 ngày trước",
      read: true,
      type: "error",
      icon: "storage",
    },
  ],
  mentor: [
    {
      id: "1",
      title: "Sinh viên nộp báo cáo",
      message: 'Nhóm "LocalHub" đã nộp báo cáo tiến độ tuần 5',
      time: "10 phút trước",
      read: false,
      type: "info",
      icon: "description",
    },
    {
      id: "2",
      title: "Yêu cầu hướng dẫn",
      message: 'Nhóm "EduPortal" yêu cầu cuộc họp vào thứ 3',
      time: "30 phút trước",
      read: false,
      type: "warning",
      icon: "event",
    },
    {
      id: "3",
      title: "Đề tài mới được phân công",
      message: "Bạn được phân công hướng dẫn thêm 1 đề tài mới",
      time: "2 giờ trước",
      read: true,
      type: "success",
      icon: "assignment_ind",
    },
    {
      id: "4",
      title: "Nhắc nhở đánh giá",
      message: "Còn 2 ngày để hoàn thành đánh giá giữa kỳ",
      time: "1 ngày trước",
      read: true,
      type: "error",
      icon: "schedule",
    },
  ],
  student: [
    {
      id: "1",
      title: "Phản hồi từ GVHD",
      message: "TS. Trần Minh Tuấn đã nhận xét về báo cáo của bạn",
      time: "15 phút trước",
      read: false,
      type: "info",
      icon: "comment",
    },
    {
      id: "2",
      title: "Lịch họp được xác nhận",
      message: "Buổi họp vào 14:00 thứ 4 đã được GVHD xác nhận",
      time: "1 giờ trước",
      read: false,
      type: "success",
      icon: "event_available",
    },
    {
      id: "3",
      title: "Deadline sắp tới",
      message: "Còn 3 ngày để nộp báo cáo tiến độ tuần 6",
      time: "3 giờ trước",
      read: false,
      type: "warning",
      icon: "alarm",
    },
    {
      id: "4",
      title: "Thông báo từ hệ thống",
      message: "Kỳ học Spring 2024 sẽ bắt đầu đăng ký vào 15/02",
      time: "2 ngày trước",
      read: true,
      type: "info",
      icon: "campaign",
    },
  ],
  evaluator: [
    {
      id: "1",
      title: "Đề tài mới cần đánh giá",
      message: "2 đề tài mới được phân công để đánh giá",
      time: "20 phút trước",
      read: false,
      type: "info",
      icon: "rate_review",
    },
    {
      id: "2",
      title: "Lịch phản biện cập nhật",
      message: 'Lịch phản biện đề tài "SmartCampus" đã được cập nhật',
      time: "1 giờ trước",
      read: false,
      type: "warning",
      icon: "calendar_month",
    },
    {
      id: "3",
      title: "Báo cáo đã được tải lên",
      message: 'Nhóm "BusDN" đã nộp báo cáo cuối kỳ',
      time: "4 giờ trước",
      read: true,
      type: "success",
      icon: "upload_file",
    },
    {
      id: "4",
      title: "Nhắc nhở deadline",
      message: "Còn 5 ngày để hoàn thành đánh giá đợt 1",
      time: "1 ngày trước",
      read: true,
      type: "error",
      icon: "timer",
    },
  ],
};

const typeColors = {
  info: { bg: "bg-blue-50", icon: "text-blue-600", border: "border-blue-200" },
  warning: { bg: "bg-amber-50", icon: "text-amber-600", border: "border-amber-200" },
  success: { bg: "bg-green-50", icon: "text-green-600", border: "border-green-200" },
  error: { bg: "bg-red-50", icon: "text-red-600", border: "border-red-200" },
};

interface NotificationDropdownProps {
  role?: UserRole;
  isNavy?: boolean;
}

export function NotificationDropdown({ role = "admin", isNavy = false }: NotificationDropdownProps) {
  const [isOpen, setIsOpen] = useState(false);
  const [notifications, setNotifications] = useState<Notification[]>(notificationsByRole[role]);
  const [dropPosition, setDropPosition] = useState<{ top: number; right: number }>({ top: 0, right: 0 });
  const dropdownRef = useRef<HTMLDivElement>(null);
  const buttonRef = useRef<HTMLButtonElement>(null);

  const unreadCount = notifications.filter((n) => !n.read).length;

  const handleToggle = () => {
    if (!isOpen && buttonRef.current) {
      const rect = buttonRef.current.getBoundingClientRect();
      setDropPosition({
        top: rect.bottom + 8,
        right: window.innerWidth - rect.right,
      });
    }
    setIsOpen((prev) => !prev);
  };

  // Close dropdown when clicking outside
  useEffect(() => {
    function handleClickOutside(event: MouseEvent) {
      if (
        dropdownRef.current &&
        !dropdownRef.current.contains(event.target as Node) &&
        buttonRef.current &&
        !buttonRef.current.contains(event.target as Node)
      ) {
        setIsOpen(false);
      }
    }
    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, []);

  const markAsRead = (id: string) => {
    setNotifications((prev) => prev.map((n) => (n.id === id ? { ...n, read: true } : n)));
  };

  const markAllAsRead = () => {
    setNotifications((prev) => prev.map((n) => ({ ...n, read: true })));
  };

  return (
    <div className="relative" ref={dropdownRef}>
      {/* Notification Bell Button */}
      <button
        ref={buttonRef}
        onClick={handleToggle}
        className={`relative transition-colors ${isNavy ? "text-blue-200 hover:text-white" : "text-slate-500 hover:text-primary"}`}
      >
        <span className="material-symbols-outlined">notifications</span>
        {unreadCount > 0 && (
          <span
            className={`absolute -top-1 -right-1 min-w-[18px] h-[18px] flex items-center justify-center text-[10px] font-bold text-white rounded-full ${isNavy ? "bg-red-500" : "bg-primary"}`}
          >
            {unreadCount}
          </span>
        )}
      </button>

      {/* Dropdown Panel */}
      <AnimatePresence>
        {isOpen && (
          <motion.div
            initial={{ opacity: 0, y: 10, scale: 0.95 }}
            animate={{ opacity: 1, y: 0, scale: 1 }}
            exit={{ opacity: 0, y: 10, scale: 0.95 }}
            transition={{ duration: 0.15 }}
            style={{ top: dropPosition.top, right: dropPosition.right }}
            className="fixed w-96 bg-white rounded-xl shadow-xl border border-slate-200 overflow-hidden z-[9999]"
          >
            {/* Header */}
            <div className="px-4 py-3 bg-slate-50 border-b border-slate-200 flex items-center justify-between">
              <div className="flex items-center gap-2">
                <span className="material-symbols-outlined text-primary text-[20px]">notifications</span>
                <h3 className="font-bold text-slate-800">Thông báo</h3>
                {unreadCount > 0 && (
                  <span className="bg-primary/10 text-primary text-xs font-bold px-2 py-0.5 rounded-full">
                    {unreadCount} mới
                  </span>
                )}
              </div>
              {unreadCount > 0 && (
                <button onClick={markAllAsRead} className="text-xs text-primary hover:text-primary-dark font-medium">
                  Đánh dấu tất cả đã đọc
                </button>
              )}
            </div>

            {/* Notification List */}
            <div className="max-h-[400px] overflow-y-auto">
              {notifications.map((notification) => (
                <motion.div
                  key={notification.id}
                  initial={{ opacity: 0 }}
                  animate={{ opacity: 1 }}
                  onClick={() => markAsRead(notification.id)}
                  className={`px-4 py-3 border-b border-slate-100 hover:bg-slate-50 cursor-pointer transition-colors ${!notification.read ? "bg-blue-50/50" : ""}`}
                >
                  <div className="flex gap-3">
                    {/* Icon */}
                    <div
                      className={`w-10 h-10 rounded-lg flex items-center justify-center flex-shrink-0 ${typeColors[notification.type].bg}`}
                    >
                      <span className={`material-symbols-outlined text-[20px] ${typeColors[notification.type].icon}`}>
                        {notification.icon}
                      </span>
                    </div>
                    {/* Content */}
                    <div className="flex-1 min-w-0">
                      <div className="flex items-start justify-between gap-2">
                        <p
                          className={`text-sm ${!notification.read ? "font-bold text-slate-800" : "font-medium text-slate-700"}`}
                        >
                          {notification.title}
                        </p>
                        {!notification.read && (
                          <span className="w-2 h-2 bg-primary rounded-full flex-shrink-0 mt-1.5" />
                        )}
                      </div>
                      <p className="text-xs text-slate-500 mt-0.5 line-clamp-2">{notification.message}</p>
                      <p className="text-[10px] text-slate-400 mt-1">{notification.time}</p>
                    </div>
                  </div>
                </motion.div>
              ))}
            </div>

            {/* Footer */}
            <div className="px-4 py-3 bg-slate-50 border-t border-slate-200">
              <button className="w-full text-center text-sm text-primary hover:text-primary-dark font-medium">
                Xem tất cả thông báo
              </button>
            </div>
          </motion.div>
        )}
      </AnimatePresence>
    </div>
  );
}
