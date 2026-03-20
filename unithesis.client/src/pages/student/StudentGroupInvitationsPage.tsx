import { useState, useEffect } from "react";
import { NotificationDropdown } from "@/components/layout";
import { SuccessModal } from "@/components/common/SuccessModal";
import { useSystemError } from "@/contexts/SystemErrorContext";
import { studentGroupService, InvitationDto } from "@/lib/studentGroupService";

export function StudentGroupInvitationsPage() {
  const { showError } = useSystemError();
  const [invitations, setInvitations] = useState<InvitationDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [acting, setActing] = useState<number | null>(null);
  const [showSuccessModal, setShowSuccessModal] = useState(false);
  const [successMessage, setSuccessMessage] = useState("");

  useEffect(() => {
    fetchInvitations();
  }, []);

  const fetchInvitations = async () => {
    try {
      setLoading(true);
      const data = await studentGroupService.getMyInvitations();
      setInvitations(data);
    } catch (err) {
      showError(err instanceof Error ? err.message : "Có lỗi khi tải dữ liệu");
      console.error("Error fetching invitations:", err);
    } finally {
      setLoading(false);
    }
  };

  const handleAccept = async (groupId: string, invitationId: number) => {
    try {
      setActing(invitationId);
      const invitation = invitations.find((i) => i.id === invitationId);
      await studentGroupService.acceptInvitation(groupId, invitationId);
      setSuccessMessage(`Đã chấp nhận lời mời từ nhóm ${invitation?.groupCode || "nhóm"}. Chúc mừng bạn!`);
      setShowSuccessModal(true);
      // Refetch after modal closes (via autocloses after 3s)
      setTimeout(() => {
        fetchInvitations();
      }, 3500);
    } catch (err) {
      showError(err instanceof Error ? err.message : "Không thể chấp nhận lời mời");
    } finally {
      setActing(null);
    }
  };

  const handleReject = async (groupId: string, invitationId: number) => {
    try {
      setActing(invitationId);
      const invitation = invitations.find((i) => i.id === invitationId);
      await studentGroupService.rejectInvitation(groupId, invitationId);
      setSuccessMessage(`Đã từ chối lời mời từ nhóm ${invitation?.groupCode || "nhóm"}.`);
      setShowSuccessModal(true);
      // Refetch after modal closes (via autocloses after 3s)
      setTimeout(() => {
        fetchInvitations();
      }, 3500);
    } catch (err) {
      showError(err instanceof Error ? err.message : "Không thể từ chối lời mời");
    } finally {
      setActing(null);
    }
  };

  const getInvitationStatus = (status: string) => {
    switch (status.toLowerCase()) {
      case "pending":
        return { label: "Chờ xử lý", color: "bg-yellow-100 text-yellow-800" };
      case "accepted":
        return { label: "Đã chấp nhận", color: "bg-green-100 text-green-800" };
      case "rejected":
        return { label: "Đã từ chối", color: "bg-red-100 text-red-800" };
      case "expired":
        return { label: "Hết hạn", color: "bg-gray-100 text-gray-800" };
      default:
        return { label: status, color: "bg-gray-100 text-gray-800" };
    }
  };

  const isPending = (invitation: InvitationDto) => invitation.status.toLowerCase() === "pending";
  const isExpired = (invitation: InvitationDto) => new Date(invitation.expiresAt) < new Date();

  return (
    <div className="flex flex-col h-screen bg-gray-50">
      {/* Header */}
      <header className="bg-white border-b border-gray-200 px-8 py-4 flex items-center justify-between">
        <h1 className="text-2xl font-bold text-gray-800">Lời mời tham gia nhóm</h1>
        <NotificationDropdown role="student" isNavy={true} />
      </header>

      {/* Main Content */}
      <main className="flex-1 overflow-y-auto p-8">
        {loading ? (
          <div className="flex items-center justify-center h-96">
            <div className="text-gray-500">Đang tải...</div>
          </div>
        ) : invitations.length === 0 ? (
          <div className="max-w-2xl mx-auto">
            <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-8 text-center">
              <div className="text-5xl mb-4">📬</div>
              <h2 className="text-2xl font-bold text-gray-800 mb-2">Không có lời mời nào</h2>
              <p className="text-gray-500">Bạn sẽ nhận được lời mời khi các nhóm khác mời bạn tham gia.</p>
            </div>
          </div>
        ) : (
          <div className="max-w-4xl mx-auto space-y-4">
            {/* Pending Invitations */}
            {invitations.some((inv) => isPending(inv)) && (
              <div>
                <h2 className="text-lg font-bold text-gray-800 mb-3">Lời mời chờ xử lý</h2>
                <div className="space-y-3 mb-6">
                  {invitations
                    .filter((inv) => isPending(inv))
                    .map((invitation) => (
                      <div
                        key={invitation.id}
                        className="bg-white border-l-4 border-yellow-500 rounded-lg shadow-sm p-6 hover:shadow-md transition"
                      >
                        <div className="flex items-start justify-between mb-4">
                          <div>
                            <h3 className="text-lg font-bold text-gray-800">
                              {invitation.groupName || invitation.groupCode}
                            </h3>
                            <p className="text-sm text-gray-600">Mã nhóm: {invitation.groupCode}</p>
                          </div>
                          <div className="text-right">
                            <span className="inline-block px-3 py-1 rounded-full text-xs font-semibold bg-yellow-100 text-yellow-800 mb-2">
                              Chờ xử lý
                            </span>
                            <p className="text-xs text-gray-500">
                              Hết hạn: {new Date(invitation.expiresAt).toLocaleDateString("vi-VN")}
                            </p>
                          </div>
                        </div>

                        <div className="mb-4">
                          <p className="text-sm text-gray-600 mb-1">
                            <span className="font-medium">Từ:</span> {invitation.inviterName}
                          </p>
                          {invitation.message && (
                            <div className="bg-gray-50 p-3 rounded-lg mt-2">
                              <p className="text-sm text-gray-700 italic">"{invitation.message}"</p>
                            </div>
                          )}
                        </div>

                        {!isExpired(invitation) ? (
                          <div className="flex gap-3">
                            <button
                              onClick={() => handleReject(invitation.groupId, invitation.id)}
                              disabled={acting === invitation.id}
                              className="flex-1 px-4 py-2 border border-red-300 text-red-700 rounded-lg font-semibold hover:bg-red-50 disabled:opacity-50 transition"
                            >
                              {acting === invitation.id ? "Đang xử lý..." : "Từ chối"}
                            </button>
                            <button
                              onClick={() => handleAccept(invitation.groupId, invitation.id)}
                              disabled={acting === invitation.id}
                              className="flex-1 px-4 py-2 bg-green-600 text-white rounded-lg font-semibold hover:bg-green-700 disabled:opacity-50 transition"
                            >
                              {acting === invitation.id ? "Đang xử lý..." : "Chấp nhận"}
                            </button>
                          </div>
                        ) : (
                          <div className="px-4 py-2 bg-gray-100 text-gray-600 rounded-lg text-center font-semibold">
                            Lời mời đã hết hạn
                          </div>
                        )}
                      </div>
                    ))}
                </div>
              </div>
            )}

            {/* Other Invitations */}
            {invitations.some((inv) => !isPending(inv)) && (
              <div>
                <h2 className="text-lg font-bold text-gray-800 mb-3">Lịch sử lời mời</h2>
                <div className="space-y-3">
                  {invitations
                    .filter((inv) => !isPending(inv))
                    .map((invitation) => {
                      const statusInfo = getInvitationStatus(invitation.status);
                      return (
                        <div
                          key={invitation.id}
                          className="bg-white border border-gray-200 rounded-lg shadow-sm p-6 opacity-75"
                        >
                          <div className="flex items-start justify-between">
                            <div>
                              <h3 className="text-lg font-bold text-gray-800">
                                {invitation.groupName || invitation.groupCode}
                              </h3>
                              <p className="text-sm text-gray-600">Mã nhóm: {invitation.groupCode}</p>
                              <p className="text-sm text-gray-500 mt-2">
                                Từ: {invitation.inviterName} •{" "}
                                {new Date(invitation.createdAt).toLocaleDateString("vi-VN")}
                              </p>
                            </div>
                            <span
                              className={`inline-block px-3 py-1 rounded-full text-xs font-semibold ${statusInfo.color}`}
                            >
                              {statusInfo.label}
                            </span>
                          </div>
                        </div>
                      );
                    })}
                </div>
              </div>
            )}
          </div>
        )}
      </main>

      {/* Success Modal */}
      <SuccessModal
        isOpen={showSuccessModal}
        onClose={() => setShowSuccessModal(false)}
        title="✨ Thành công"
        message={successMessage}
        icon="check_circle"
        autoClose={3000}
      />
    </div>
  );
}
